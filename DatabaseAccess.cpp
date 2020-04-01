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
	return std::list<Album>();
}

const std::list<Album> DatabaseAccess::getAlbumsOfUser(const User& user)
{
	return std::list<Album>();
}

void DatabaseAccess::createAlbum(const Album& album)
{
}

void DatabaseAccess::deleteAlbum(const std::string& albumName, int userId)
{
}

bool DatabaseAccess::doesAlbumExists(const std::string& albumName, int userId)
{
	return false;
}

Album DatabaseAccess::openAlbum(const std::string& albumName)
{
	return Album();
}

void DatabaseAccess::closeAlbum(Album& pAlbum)
{
}

void DatabaseAccess::printAlbums()
{
}


/*			Picture related				*/
void DatabaseAccess::addPictureToAlbumByName(const std::string& albumName, const Picture& picture)
{
}

void DatabaseAccess::removePictureFromAlbumByName(const std::string& albumName, const std::string& pictureName)
{
}

void DatabaseAccess::tagUserInPicture(const std::string& albumName, const std::string& pictureName, int userId)
{
}

void DatabaseAccess::untagUserInPicture(const std::string& albumName, const std::string& pictureName, int userId)
{
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
