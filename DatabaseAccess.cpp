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
	std::string condition = "WHERE User_id = " + std::to_string(user.getId());
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
		std::string sqlStat = "INSERT INTO Albums (Name, User_id, Creation_date) VALUES ('" + album.getName() + "', " + userId + ", '" + album.getCreationDate() + "');";
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
	std::string sqlStat;
	if (albumName.size() > 0) // If albu, name is empty, delete all albums of user.
	{
		Album albumToDelete = this->openAlbum(albumName);
		// Delete all pictures&tags from album.
		this->removePictureFromAlbumByName(albumName, "");

		sqlStat = "DELETE FROM Albums WHERE Name = '" + albumName + "' User_id = " + std::to_string(userId) + ';';
	}
	else
	{
		// Get all user albums.
		std::list<Album> userAlbums = this->getAlbumsOfUser(this->getUser(userId));

		// Delete all pictures & tags from all albums of user.
		for (Album album : userAlbums)
		{
			this->removePictureFromAlbumByName(album.getName(), "");
		}

		sqlStat = "DELETE FROM Albums WHERE User_id = " + std::to_string(userId) + ';';
	}

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

	// Check if there is album with that name.
	std::string condition = "WHERE Name = '" + albumName + "'";
	this->_selectFunc("*", "Albums", condition, &selectData);

	// If enter to loop, There is albu, with tha same name.
	for (std::unordered_map<std::string, std::string> row : selectData)
	{
			return true;
	}

	return false;
}

Album DatabaseAccess::openAlbum(const std::string& albumName)
{
	SELECT_DATA selectData;
	Album openAlbum;

	// Get album data
	std::string condition = "WHERE Name = '" + albumName + "'";
	this->_selectFunc("*", "Albums", condition, &selectData);

	openAlbum.setCreationDate(selectData[0]["Creation_date"]);
	openAlbum.setName(selectData[0]["Name"]);
	openAlbum.setOwner(stoi(selectData[0]["User_id"]));

	// Get all photos of album
	condition = "WHERE Album_id = " + selectData[0]["ID"];
	this->_selectFunc("*", "Pictures", condition, &selectData);

	for (std::unordered_map<std::string, std::string> row : selectData)
	{
		// Create picture
		int picId = stoi(row["ID"]);
		std::string picName = row["Name"];
		std::string picLocation = row["Location"];
		std::string picDate = row["Creation_date"];
		Picture pic(picId, picName, picLocation, picDate);
		
		// Add all tags of picture.
		SELECT_DATA tagsDataSelect;
		condition = "WHERE Picture_id = " + row["ID"];
		this->_selectFunc("*", "Tags", condition, &tagsDataSelect);
		
		for (std::unordered_map<std::string, std::string> tagRow : tagsDataSelect)
		{
			int userId = stoi(tagRow["User_id"]);
			pic.tagUser(userId);
		}

		// Add new picture
		openAlbum.addPicture(pic);
	}

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
		std::cout << "	~ Album list is empty. " << std::endl;
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
		std::string pictureName = "'" + picture.getName() + "', ";
		std::string pictueId = std::to_string(picture.getId()) + ", ";

		// Get album id.
		this->_selectFunc("ID", "Albums", "WHERE Name = '" + albumName + "'", &selectData);
		albumID = selectData[0]["ID"];

		// Add new picture
		std::string sqlStat = "INSERT INTO Pictures (ID, Name, Location, Creation_date, Album_id) VALUES (" + pictueId + pictureName + location + creationDate + albumID + ");";
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
		this->_selectFunc("ID", "Albums", "WHERE Name = '" + albumName + "'", &selectData);
		std::string albumID = selectData[0]["ID"];

		std::string sqlStat = "";
		if (pictureName.size() > 0)
		{
			// Delete from Picture
			untagUserInPicture(albumName, pictureName, UNTAG_ALL);
			sqlStat = "DELETE FROM Pictures WHERE Name = '" + pictureName + "' AND Album_id = " + albumID + ";";
		}
		else // If pictureName is not empty, delete all photos from album.
		{
			// Get all pictures on album.
			this->_selectFunc("ID", "Pictures", "WHERE Album_id = " + albumID, &selectData);

			// Delete all tags from all the pictures in album.
			for (std::unordered_map<std::string, std::string> row : selectData)
			{
				this->untagUserInPicture(albumName, row["Name"], UNTAG_ALL);
			}

			sqlStat = "DELETE FROM Pictures WHERE Album_id = " + albumID + ";";
		}

		// Delete picture from album.
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
		this->_selectFunc("ID", "Albums", "WHERE Name = '" + albumName + "'", &selectData);
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
		std::string sqlStat;
		if (albumName.size() > 0 && pictureName.size() > 0) // If want to delete all user tags - get only user id.
		{
			SELECT_DATA selectData;

			// Get album id.
			this->_selectFunc("ID", "Albums", "WHERE Name = '" + albumName + "'", &selectData);
			std::string albumID = selectData[0]["ID"];

			// Get picture id. 
			this->_selectFunc("ID", "Pictures", "WHERE Name = '" + pictureName + "' AND Album_id = " + albumID, &selectData);
			std::string pictureID = selectData[0]["ID"];


			// Delete tag
			if (userId == UNTAG_ALL) // If want to delete all tag of picture.
				sqlStat = "DELETE FROM Tags WHERE Picture_id = " + pictureID + ";";
			else
				sqlStat = "DELETE FROM Tags WHERE Picture_id = " + pictureID + " AND User_id = " + std::to_string(userId) + ";";
		}
		else
		{
			sqlStat = "DELETE FROM Tags WHERE User_id = " + std::to_string(userId) + ";";
		}

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
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		SELECT_DATA selectData;

		// Get all users
		this->_selectFunc("*", "Users", "", &selectData);

		// Print users list.
		std::cout << "Users list:" << std::endl;
		std::cout << "-----------" << std::endl;
		for (std::unordered_map<std::string, std::string> row : selectData)
		{
			std::cout << "	+ @" << row["ID"] << " - " << row["Name"] << std::endl;
		}
		if (selectData.size() < 1)
			std::cout << "	~ Users list is empty." << std::endl;

	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
}

User DatabaseAccess::getUser(int userId)
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		SELECT_DATA selectData;
		// Get user
		this->_selectFunc("*", "Users", "WHERE ID = " + std::to_string(userId), &selectData);
		return User(stoi(selectData[0]["ID"]), selectData[0]["Name"]);
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
}

void DatabaseAccess::createUser(User& user)
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		// Add new user
		std::string sqlStat = "INSERT INTO Users (Name, ID) VALUES ('" + user.getName() + "', " + std::to_string(user.getId()) + ");";
		char* errMesg = nullptr;

		int res = sqlite3_exec(this->_database, sqlStat.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK)
		{
			throw MyException("Error adding new user ");
		}
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
}

void DatabaseAccess::deleteUser(const User& user)
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		// Delete all albums of user.
		this->deleteAlbum("", user.getId());
		
		// Delete all tags of user.
		this->untagUserInPicture("", "", user.getId());

		std::string sqlStat = "DELETE FROM Users Where ID = " + std::to_string(user.getId());
		char* errMesg = nullptr;
		int res = sqlite3_exec(this->_database, sqlStat.c_str(), nullptr, nullptr, &errMesg);
		if (res != SQLITE_OK)
		{
			throw MyException("Error deleting user");
		}
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
}

bool DatabaseAccess::doesUserExists(int userId)
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		SELECT_DATA selectData;
		
		// If the data isnt empty the user exist.
		this->_selectFunc("ID", "Users", "WHERE ID = " + std::to_string(userId), &selectData);		
		for (std::unordered_map<std::string, std::string> row : selectData)
		{
			return true;
		}

		return false;
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
	return false;
}


/*			User statistics				*/
int DatabaseAccess::countAlbumsOwnedOfUser(const User& user)
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		SELECT_DATA selectData;

		// Count
		this->_selectFunc("count(ID)", "albums", "WHERE User_id = " + std::to_string(user.getId()), &selectData);
		int count = stoi(selectData[0]["count(ID)"]);
		return count;
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
	
	return 0;
}

int DatabaseAccess::countAlbumsTaggedOfUser(const User& user)
{
	int count = 0;

	// Get all albums
	std::list<Album> albums = this->getAlbums();

	// Go through all the albums
	for (Album album : albums)
	{
		// Get all the pictures
		std::list<Picture> pictures = album.getPictures();

		// Check if user tagged in one pic of the album.
		for (Picture pic : pictures)
		{
			if (pic.isUserTagged(user))
			{
				count += 1;
				break;
			}
		}
	}

	return count;
}

int DatabaseAccess::countTagsOfUser(const User& user)
{
	int count = 0;

	// Get all albums
	std::list<Album> albums = this->getAlbums();
	
	// Go through all the albums
	for (Album album : albums)
	{
		// Get all photos & go though them.
		std::list<Picture> pictures = album.getPictures();
		for (Picture pic : pictures)
		{
			if (pic.isUserTagged(user)) // If the user is tagged add 1.
				count += 1;
		}
	}

	return count;
}

float DatabaseAccess::averageTagsPerAlbumOfUser(const User& user)
{
	int albumsAmount = countAlbumsOwnedOfUser(user), totalTags = 0;
	// Get all albums of user
	std::list<Album> albumsUser = this->getAlbumsOfUser(user);

	// Go through albums & get all the tags.
	for (Album album : albumsUser)
	{
		std::list<Picture> pictures = album.getPictures();
		for (Picture pic : pictures)
		{
			totalTags += pic.getTagsCount();
		}
	}

	// Get average of tags per album.
	return (albumsAmount == 0) ? 0 : (totalTags / albumsAmount);
}


/*			Queries			*/
User DatabaseAccess::getTopTaggedUser()
{
	if (_access(this->_dbName.c_str(), 0) == 0)
	{
		SELECT_DATA selectData;
		int top = 0;
		User topUser(0, "");

		// Get all users
		this->_selectFunc("*", "Users", "", &selectData);

		// Go thought users list
		for (std::unordered_map<std::string, std::string> row : selectData)
		{
			User curr = getUser(stoi(row["ID"]));
			int currTags = countTagsOfUser(curr);
			if (currTags >= top)
			{
				top = currTags;
				topUser = curr;
			}
		}

		// If there are no users.
		if (topUser.getId() == 0)
			throw MyException("There is no 'Top Tagged user'.");

		return topUser;
	}
	else
	{
		throw MyException("Can't find DataBase.");
	}
}

Picture DatabaseAccess::getTopTaggedPicture()
{
	int topTaggs = 0;
	Picture topPhoto(0, "");

	// Get all albums
	std::list<Album> albums = this->getAlbums();

	// Go through all the albums
	for (Album album : albums)
	{
		// Get all the pictures
		std::list<Picture> pictures = album.getPictures();

		// Check if user tagged in one pic of the album.
		for (Picture pic : pictures)
		{
			if (pic.getTagsCount() >= topTaggs)
			{
				topPhoto = pic;
				topTaggs = pic.getTagsCount();
			}
		}
	}

	// If there are no photos.
	if (topPhoto.getId() == 0)
		throw MyException("There is no 'Top Tagged picture'.");

	return topPhoto;
}

std::list<Picture> DatabaseAccess::getTaggedPicturesOfUser(const User& user)
{
	std::list<Picture> photosList;

	// Get all albums
	std::list<Album> albums = this->getAlbums();

	// Go through all the albums
	for (Album album : albums)
	{
		// Get all the pictures
		std::list<Picture> pictures = album.getPictures();

		// Check if user tagged, if yes add the photo to the list.
		for (Picture pic : pictures)
		{
			if (pic.isUserTagged(user))
			{
				photosList.push_front(pic);
			}
		}
	}

	return photosList;
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
		// Clear data
		data->clear();

		std::string sqlStat = "SELECT " + field + " FROM " + table + " " + condition + ";";
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