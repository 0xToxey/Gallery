#pragma once
#include "IDataAccess.h"
#include <iostream>
#include <io.h>
#include <unordered_map>
#include "sqlite3.h"
#include "MyException.h"
#include "AlbumNotOpenException.h"
#include "ItemNotFoundException.h"


#define SELECT_DATA std::vector<std::unordered_map<std::string, std::string>>

class DatabaseAccess : public IDataAccess
{
public:
	DatabaseAccess();
	~DatabaseAccess();

	// album related
	virtual const std::list<Album> getAlbums();
	virtual const std::list<Album> getAlbumsOfUser(const User& user);
	virtual void createAlbum(const Album& album);
	virtual void deleteAlbum(const std::string& albumName, int userId);
	virtual bool doesAlbumExists(const std::string& albumName, int userId);
	virtual Album openAlbum(const std::string& albumName);
	virtual void closeAlbum(Album& pAlbum);
	virtual void printAlbums();

	// picture related
	virtual void addPictureToAlbumByName(const std::string& albumName, const Picture& picture);
	virtual void removePictureFromAlbumByName(const std::string& albumName, const std::string& pictureName);
	virtual void tagUserInPicture(const std::string& albumName, const std::string& pictureName, int userId);
	virtual void untagUserInPicture(const std::string& albumName, const std::string& pictureName, int userId);

	// user related
	virtual void printUsers();
	virtual User getUser(int userId);
	virtual void createUser(User& user);
	virtual void deleteUser(const User& user);
	virtual bool doesUserExists(int userId);


	// user statistics
	virtual int countAlbumsOwnedOfUser(const User& user);
	virtual int countAlbumsTaggedOfUser(const User& user);
	virtual int countTagsOfUser(const User& user);
	virtual float averageTagsPerAlbumOfUser(const User& user);

	// queries
	virtual User getTopTaggedUser();
	virtual Picture getTopTaggedPicture();
	virtual std::list<Picture> getTaggedPicturesOfUser(const User& user);

	// db Handle - V
	virtual bool open();
	virtual void close();
	virtual void clear();

private:
	sqlite3* _database;
	std::string _dbName = "GalleryDB.sqlite";
	
	void _selectFunc(std::string field, std::string table, std::string condition, SELECT_DATA* data);
	friend int _selectRequest(void* data, int argc, char** argv, char** azColName);
};