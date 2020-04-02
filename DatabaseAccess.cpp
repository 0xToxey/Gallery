#pragma once
#include "DatabaseAccess.h"


DatabaseAccess::DatabaseAccess() : _database(nullptr)
{
}

DatabaseAccess::~DatabaseAccess()
{
}


/*			Album related			*/
const std::list<Album> DatabaseAccess::getAlbums()
{
	SELECT_DATA selectData;
	std::list<Album> albumList;

	// Get all albums
	this->_selectFunc("*", "Albums", "", &selectData);

	// Create the albums List
	for (std::unordered_map<std::string, std::string> row : selectData)
	{
		albumList.push_back(openAlbum(row["Name"]));
	}

	return albumList;
}

const std::list<Album> DatabaseAccess::getAlbumsOfUser(const User& user)
{
	SELECT_DATA selectData;
	std::list<Album> albumList;
	
	// Get all albums of user.
	std::string condition = "WHERE ID = " + user.getId();
	this->_selectFunc("*", "Albums", condition, &selectData);

	// Create the albums List
	for (std::unordered_map<std::string, std::string> row : selectData)
	{
		albumList.push_back(openAlbum(row["Name"]));
	}

	return albumList;
}

void DatabaseAccess::createAlbum(const Album& album)
{
	int doesFileExist = _access(this->_dbName.c_str(), 0);

	if (doesFileExist == 0)
	{
		// Insert into PhonePrefix
		std::string userId = std::to_string(album.getOwnerId());
		std::string sqlStat = "INSERT INTO Albums (Name, User_id, Creation_date) VALUES (" + album.getName() + " " + userId + album.getCreationDate() + ");";
		char* errMesg = nullptr;

		int res = sqlite3_exec(this->_database, sqlStat.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK)
		{
			throw MyException("Error INSERT into Albums");
		}
	}
	else
		throw MyException("Can't find DataBase.");
}

void DatabaseAccess::deleteAlbum(const std::string& albumName, int userId)
{
	std::string sqlStat = "DELETE FROM Albums WHERE User_id = " + userId + ';';
	char* errMesg = nullptr;

	int res = sqlite3_exec(this->_database, sqlStat.c_str(), nullptr, nullptr, &errMesg);
	if (res != SQLITE_OK)
	{
		throw MyException("Error deleting album");
	}
}

bool DatabaseAccess::doesAlbumExists(const std::string& albumName, int userId)
{
	SELECT_DATA selectData;

	// Get all albums of user.
	std::string condition = "WHERE User_id = " + std::to_string(userId) + " and name = '" + albumName + "'";
	this->_selectFunc("*", "Albums", condition, &selectData);

	// Create the albums List
	for (std::unordered_map<std::string, std::string> row : selectData)
	{
		if (row["User_id"] == std::to_string(userId))
			return true;
	}

	return false;
}

Album DatabaseAccess::openAlbum(const std::string& albumName)
{
	SELECT_DATA selectData;
	Album openAlbum;

	// Get album data
	std::string condition = "WHERE name = '" + albumName + "'";
	this->_selectFunc("*", "Albums", condition, &selectData);

	openAlbum.setCreationDate(selectData[0]["Creation_date"]);
	openAlbum.setName(selectData[0]["Name"]);
	openAlbum.setOwner(stoi(selectData[0]["User_id"]));

	return openAlbum;
}

void DatabaseAccess::closeAlbum(Album& pAlbum)
{
}

void DatabaseAccess::printAlbums()
{
	std::list<Album> albumList = getAlbums();

	// Print the albums list
	std::cout << "Album list:" << std::endl;
	std::cout << "-----------" << std::endl;
	for (Album album : albumList)
	{
		std::cout << "	* [" << album.getName() << "] - created by " << getUser(album.getOwnerId()).getName() << "@" << album.getOwnerId() << " on " << album.getCreationDate() << std::endl;
	}
	 
	//	If album list is empty.
	if (albumList.size() == 0)
	{
		std::cout << "Album list is empty. " << std::endl;
	}
}


/*			Picture related				*/
void DatabaseAccess::addPictureToAlbumByName(const std::string& albumName, const Picture& picture)
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		SELECT_DATA selectData;
		std::string albumID;
		std::string creationDate = "'" + picture.getCreationDate() + "', ";
		std::string location = "'" + picture.getPath() + "', ";
		std::string name = "'" + picture.getName() + "', ";

		// Get album id.
		this->_selectFunc("ID", "Albums", "WHERE Name = " + albumName, &selectData);
		albumID = selectData[0]["ID"];

		// Add new picture
		std::string sqlStat = "INSERT INTO Pictures (Name, Location, Creation_date, Album_id) VALUES (" + name + location + creationDate + albumID + ");";
		char* errMesg = nullptr;

		int res = sqlite3_exec(this->_database, sqlStat.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK)
		{
			throw MyException("Error INSERT to " + albumName);
		}
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
}

void DatabaseAccess::removePictureFromAlbumByName(const std::string& albumName, const std::string& pictureName)
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		// Get album id.
		SELECT_DATA selectData;
		this->_selectFunc("ID", "Albums", "WHERE Name = " + albumName, &selectData);
		std::string albumID = selectData[0]["ID"];

		// Delete from Picture
		untagUserInPicture(albumName, pictureName, UNTAG_ALL);
		std::string sqlStat = "DELETE FROM Pictures WHERE Name = '" + pictureName + "' AND Album_id = " + albumID  + ";";
		char* errMesg = nullptr;

		int res = sqlite3_exec(this->_database, sqlStat.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK)
		{
			throw MyException("Error DELETE from " + albumName);
		}
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
}

void DatabaseAccess::tagUserInPicture(const std::string& albumName, const std::string& pictureName, int userId)
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		SELECT_DATA selectData;

		// Get album id.
		this->_selectFunc("ID", "Albums", "WHERE Name = " + albumName, &selectData);
		std::string albumID = selectData[0]["ID"];

		// Get picture id. 
		this->_selectFunc("ID", "Pictures", "WHERE Name = '" + pictureName + "' AND Album_id = " + albumID, &selectData);
		std::string pictureID = selectData[0]["ID"] + ", ";


		// Add new tag 
		std::string sqlStat = "INSERT INTO Tags (Picture_id, User_id) VALUES (" + pictureID + std::to_string(userId) + ");";
		char* errMesg = nullptr;

		int res = sqlite3_exec(this->_database, sqlStat.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK)
		{
			throw MyException("Error TAG on " + albumName);
		}
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
}

void DatabaseAccess::untagUserInPicture(const std::string& albumName, const std::string& pictureName, int userId)
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		SELECT_DATA selectData;

		// Get album id.
		this->_selectFunc("ID", "Albums", "WHERE Name = " + albumName, &selectData);
		std::string albumID = selectData[0]["ID"];

		// Get picture id. 
		this->_selectFunc("ID", "Pictures", "WHERE Name = '" + pictureName + "' AND Album_id = " + albumID, &selectData);
		std::string pictureID = selectData[0]["ID"] + ", ";


		// Delete tag
		std::string sqlStat;
		if (userId == UNTAG_ALL) // If want to delete all tag of picture.
			sqlStat = "DELETE FROM Tags WHERE Picture_id = " + pictureID + ");";
		else
			sqlStat = "DELETE FROM Tags WHERE Picture_id = " + pictureID + "AND User_id = " + std::to_string(userId) + ");";

		char* errMesg = nullptr;

		int res = sqlite3_exec(this->_database, sqlStat.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK)
		{
			throw MyException("Error delete tags.");
		}
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
}


/*			User related			*/
void DatabaseAccess::printUsers()
{
}

User DatabaseAccess::getUser(int userId)
{
	return User(0, "null");;
}

void DatabaseAccess::createUser(User& user)
{
}

void DatabaseAccess::deleteUser(const User& user)
{
}

bool DatabaseAccess::doesUserExists(int userId)
{
	return false;
}


/*			User statistics				*/
int DatabaseAccess::countAlbumsOwnedOfUser(const User& user)
{
	return 0;
}

int DatabaseAccess::countAlbumsTaggedOfUser(const User& user)
{
	return 0;
}

int DatabaseAccess::countTagsOfUser(const User& user)
{
	return 0;
}

float DatabaseAccess::averageTagsPerAlbumOfUser(const User& user)
{
	return 0.0f;
}


/*			Queries			*/
User DatabaseAccess::getTopTaggedUser()
{
	return User(0,"null");
}

Picture DatabaseAccess::getTopTaggedPicture()
{
	return Picture(0, "null");
}

std::list<Picture> DatabaseAccess::getTaggedPicturesOfUser(const User& user)
{
	return std::list<Picture>();
}


/*			DataBase handle			*/
bool DatabaseAccess::open()
{
	// Try open DataBase.
	int res = sqlite3_open(this->_dbName.c_str(), &this->_database); 
	if (res != SQLITE_OK)
	{
		this->_database = nullptr;
		std::cout << "Failed to open DataBase" << std::endl;
		return false;
	}

	// Create tables if doesnt exist.
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		// Users Table
		std::string sqlCommand = "CREATE TABLE IF NOT EXISTS Users (ID INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL , Name TEXT NOT NULL);";
		char* errMesg = nullptr;
		int res = sqlite3_exec(this->_database, sqlCommand.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK) // If could not create DB.
		{
			throw MyException("Error creating DataBase -> " + std::string(errMesg));
		}

		// Albums Table
		sqlCommand = "CREATE TABLE IF NOT EXISTS Albums (ID INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL , Name TEXT NOT NULL , User_id INTEGER NOT NULL , Creation_date TEXT NOT NULL,  FOREIGN KEY(User_id) REFERENCES USERS (ID));";
		errMesg = nullptr;
		res = sqlite3_exec(this->_database, sqlCommand.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK) // If could not create DB.
		{
			throw MyException("Error creating DataBase -> " + std::string(errMesg));
		}

		// Pictures Table
		sqlCommand = "CREATE TABLE IF NOT EXISTS Pictures( ID  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,  Name TEXT  NOT NULL, Location TEXT NOT NULL, Creation_date TEXT NOT NULL, Album_id INTEGER NOT NULL,  FOREIGN KEY(Album_id) REFERENCES Albums (ID));";
		errMesg = nullptr;
		res = sqlite3_exec(this->_database, sqlCommand.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK) // If could not create DB.
		{
			throw MyException("Error creating DataBase -> " + std::string(errMesg));
		}

		// Tags Table
		sqlCommand = "CREATE TABLE IF NOT EXISTS Tags(ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Picture_id INTEGER NOT NULL, User_id INTEGER NOT NULL, FOREIGN KEY(Picture_id) REFERENCES PICTURES (ID), FOREIGN KEY(User_id) REFERENCES USERS (ID));";
		errMesg = nullptr;
		res = sqlite3_exec(this->_database, sqlCommand.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK) // If could not create DB.
		{
			throw MyException("Error creating DataBase -> " + std::string(errMesg));
		}
	}
	else
	{
		throw MyException("Can't find DataBase. ");
	}

	return true;
}

void DatabaseAccess::close()
{
	sqlite3_close(this->_database);
	this->_database = nullptr;
}

void DatabaseAccess::clear()
{
}


/*
	Function helps with the select request.
*/
void DatabaseAccess::_selectFunc(std::string field, std::string table, std::string condition, SELECT_DATA* data) const
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		// std::string sqlStat = "SELECT * FROM ALBUMS;";
		std::string sqlStat = "SELECT " + field + " FROM " + table + condition + ";";
		char* errMesg = nullptr;

		int res = sqlite3_exec(this->_database, sqlStat.c_str(), _selectRequest, data, &errMesg);
		if (res != SQLITE_OK)
		{
			throw MyException("Error Getting data from " + table);
		}
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
}

/*
CallBack Funtion.
*/
int _selectRequest(void* data, int argc, char** argv, char** azColName)
{
	SELECT_DATA* table = static_cast<SELECT_DATA*>(data);
	std::unordered_map<std::string, std::string> row;

	for (int i = 0; i < argc; i++)
	{
		row[azColName[i]] = argv[i]; // Get the data
	}

	table->push_back(row);		// Push to the vector
	return 0;
}