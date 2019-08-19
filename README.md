# FileSort
Windows tool for sorting alphabetically all the files inside a folder based on its structure. Download latest version ![here](/Build/)

Media players - including in-car entertainment systems, like newer versions of the BMW iDrive - handle music collections in a simple way. Everything is mixed together and presented alphabetically, if no filter like artist or genre is applied. This makes listening to music hard:

- when the whole collection is presented alphabetically, all files are mixed together. Considering most albums prefix the tracks with numbers, the whole list will contain all the 01s followed by the 02s and so on, which makes listening to a certain album extremely hard (especially in cars).
- when a certain artist or genre is selected, tracks which don't contain metadata (i.e. ID3) or have cryptic names instead of the artist/track, will normally not be presented.

FileSort solves both of these issues by gathering all files found in the source folder and prefixing them with a number. This way, the music collection will be presented exactly like the folder tree structure: all files are used and they remain grouped together. Another great benefit is that the music collection can be sorted after genre, artist or purchase year or every other criteria prior to importing it, depending on personal preference.

Note: FileSort should be run on the NTFS file system, to ensure proper sorting. FAT could return different results, depending on how the source folder was created. This is because of the way Win32 API works.

FileSort offers 3 options when processing a source folder:
- *Rename the original file*: every file will be renamed to have an index prepended.
- *Copy the sorted file next to the original one*: the prepended index will be applied to a copy of the file and the copy will reside next to the original file
- *Copy the source file into the source folder*: a new folder *FileSort* will be created inside the source folder and all indexed copies will be placed there. This is the recommended option for sorting a collection that will be used inside a car. 

## License
FileSort is licensed under the MIT license.

## Contact
Cristian Sava <<cristianzsava@gmail.com>>
