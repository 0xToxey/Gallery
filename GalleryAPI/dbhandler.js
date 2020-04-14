var sqlite3 = require('sqlite3').verbose()
const DataBaseSource = "./GalleryDB.sqlite"

let db = new sqlite3.Database(DataBaseSource, (err) => {
    if (err)
    {
        console.error(err.message)
        throw err
    }
    else
    {
        console.log("Connected to DataBase.")
        db.run("CREATE TABLE IF NOT EXISTS Users (ID INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL , Name TEXT NOT NULL, Password TEXT NOT NULL)", (err) => {if (err) {console.error(err.message); throw err;}});
        db.run("CREATE TABLE IF NOT EXISTS Albums (ID INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL , Name TEXT NOT NULL , User_id INTEGER NOT NULL , Creation_date TEXT NOT NULL,  FOREIGN KEY(User_id) REFERENCES USERS (ID) ON DELETE CASCADE)", (err) => {if (err) {console.error(err.message); throw err;}});
        db.run("CREATE TABLE IF NOT EXISTS Pictures( ID  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,  Name TEXT  NOT NULL, Location TEXT NOT NULL, Creation_date TEXT NOT NULL, Album_id INTEGER NOT NULL,  FOREIGN KEY(Album_id) REFERENCES Albums (ID) ON DELETE CASCADE)", (err) => {if (err) {console.error(err.message); throw err;}});
        db.run("CREATE TABLE IF NOT EXISTS Tags(ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Picture_id INTEGER NOT NULL, User_id INTEGER NOT NULL, FOREIGN KEY(Picture_id) REFERENCES PICTURES (ID) ON DELETE CASCADE, FOREIGN KEY(User_id) REFERENCES USERS (ID) ON DELETE CASCADE)", (err) => {if (err) {console.error(err.message); throw err;}});
    }
});

module.exports = db