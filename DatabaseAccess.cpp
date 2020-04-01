#pragma once
#include "DatabaseAccess.h"

DatabaseAccess::DatabaseAccess()
{
}

DatabaseAccess::~DatabaseAccess()
{
}

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

void DatabaseAccess::printUsers()
{
}

User DatabaseAccess::getUser(int userId)
{
	User a = User(15, "a");
	return a;
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

bool DatabaseAccess::open()
{
	return false;
}

void DatabaseAccess::close()
{
}

void DatabaseAccess::clear()
{
}
