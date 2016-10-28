# BinFileWriter
C# Class to read/write files in binary with choosen ecnryption

## Add BinFileWriter.dll to project references
## Instantiate the Class

var bfw = new BinFileWriter();

## Set class required parameters

bfw.initClass(object obj,string filePath,int pathType, int encryptType, string encryptKey, int fileEncryptType, int maxRecords);

object obj		        - Type of Object to write (An array of this type will be created)
string filePath			- Path to read/write file
int pathType			- Path type (0 - Hard Disk 1 - Shared Drive 2 - HTTP 3 - HTTPS 4 - FTP 5 - SFTP)
int encryptType 		- Key encryption type
string encryptKey		- Key to encrypt file
int fileEncryptType 	        - File encryption type (0 - DES 1 - AES)
int maxRecords			- Max records, used to instantiate the array of objects

## Add data to the array
bfw.addTo(object data);

## Write array to file
var result = null;
if(!result = bfw.writeFile())
	// Error
	
## Read file to array
var result = null;
if(!result = bfw.readFile())
	// Error
else
	var fileContent = result
	
	

## The flow of this class is to create an array of objects with the type defined in object obj in initClass.
## Then the user should use addTo(object data) to add more records to that array and then use writeFile()
## to write the array encrypted to a file specifiec in string filePath in iniClass.
## When using the class to read files, an array is returned with all the objects previous added readFile()
## the memory array is also set with this content so the user can continue to add objects. Note that for every
## new record, sizeAlteration(int maxRecords) must be called first to expand the array size, user can expand the
## array by any size. This method allocates a new array with the specified size, copies elements from the old 
## array to the new one, and then replaces the old array with the new one. Please note that the running machine
## should have enought memory to allocate both arrays


