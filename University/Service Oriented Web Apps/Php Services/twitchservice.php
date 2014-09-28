<?php
header('Content-type:  text/xml');
require 'database.php';

// Call requested function - based off: http://stackoverflow.com/a/4360220
if(isset($_GET['AllTwitches']))
{
	AllTwitches();
}
if(isset($_GET['TwitchByID']) && isset($_GET['id']))
{
	TwitchByID($_GET['id']);
}
if(isset($_GET['TwitchBySpecies']) && isset($_GET['name']))
{
	TwitchBySpecies($_GET['name']);
}
if(isset($_GET['TwitchByLocation']) && isset($_GET['location']))
{
	TwitchByLocation($_GET['location']);
}
if(isset($_GET['TwitchSearch']) && isset($_GET['species']) && isset($_GET['location']) && isset($_GET['hasImage']))
{
	TwitchSearch($_GET['species'], $_GET['location'], $_GET['hasImage']);
}
if(isset($_GET['TwitchSearchXSLT']) && isset($_GET['species']) && isset($_GET['location']) && isset($_GET['hasImage']) && isset($_GET['xslt']))
{
	TwitchSearchXSLT($_GET['species'], $_GET['location'], $_GET['hasImage'], $_GET['xslt']);
}
if(isset($_GET['GetSpecies']) && isset($_GET['name']))
{
	GetSpecies($_GET['name']);
}
if(isset($_GET['GetLocation']) && isset($_GET['location']))
{
	GetLocations($_GET['location']);
}

function AllTwitches()
{
	$link = db_connect();
	$query = "SELECT * FROM Twitch JOIN User ON Twitch.id_user=User.uid_user";
	$result = mysqli_query($link,$query);
	echo CreateTwitchResult($result, null);
}

function TwitchByID($id)
{
	$link = db_connect();
	$query = "SELECT * FROM Twitch JOIN User ON Twitch.id_user=User.uid_user WHERE uid_twitch ='$id'";
	$result = mysqli_query($link,$query);
	echo CreateTwitchResult($result, null);
}

function TwitchBySpecies($name)
{
	$link = db_connect();
	$query = "SELECT * FROM Twitch JOIN User ON Twitch.id_user=User.uid_user WHERE name LIKE '%{$name}%'";
	$result = mysqli_query($link,$query);
	echo CreateTwitchResult($result, null);
}

function TwitchByLocation($location)
{
	$link = db_connect();
	$query = "SELECT * FROM Twitch JOIN User ON Twitch.id_user=User.uid_user WHERE location LIKE '%{$location}%'";
	$result = mysqli_query($link,$query);
	echo CreateTwitchResult($result, null);
}

function TwitchSearch($species, $location, $hasImage)
{
	$link = db_connect();
	$query;
	if($hasImage == 2)
		$query = "SELECT * FROM Twitch JOIN User ON Twitch.id_user=User.uid_user WHERE name LIKE '%{$species}%' AND location LIKE '%{$location}%'";
	else
		$query = "SELECT * FROM Twitch JOIN User ON Twitch.id_user=User.uid_user WHERE name LIKE '%{$species}%' AND location LIKE '%{$location}%' AND hasImage='$hasImage'";
	$result = mysqli_query($link,$query);
	echo CreateTwitchResult($result, null);
}

function TwitchSearchXSLT($species, $location, $hasImage, $xslt)
{
	$link = db_connect();
	$query;
	if($hasImage == 2)
		$query = "SELECT * FROM Twitch JOIN User ON Twitch.id_user=User.uid_user WHERE name LIKE '%{$species}%' AND location LIKE '%{$location}%'";
	else
		$query = "SELECT * FROM Twitch JOIN User ON Twitch.id_user=User.uid_user WHERE name LIKE '%{$species}%' AND location LIKE '%{$location}%' AND hasImage='$hasImage'";
	$result = mysqli_query($link,$query);
	echo CreateTwitchResult($result, $xslt);
}

function GetSpecies($species)
{
	$link = db_connect();
	$query = "SELECT * FROM Twitch WHERE name LIKE '%{$species}%'";
	$result = mysqli_query($link,$query);
	echo CreateNameResult($result, 'species');
}

function GetLocations($location)
{
	$link = db_connect();
	$query = "SELECT * FROM Twitch WHERE location LIKE '%{$location}%'";
	$result = mysqli_query($link,$query);
	echo CreateNameResult($result, 'location');
}

function CreateTwitchResult($result, $xslt)
{
	$xmlDom = new DOMDocument();
	if($xslt != null)
		$xmlDom->appendChild($xmlDom->createProcessingInstruction('xml-stylesheet', 'href="' . $xslt . '.xsl" type="text/xsl"'));
	$xmlDom->appendChild($xmlDom->createElement('twitches'));
	$xmlRoot = $xmlDom->documentElement;
	if ( mysqli_num_rows($result) > 0 ) {
	   while ( $row = mysqli_fetch_array($result) ) {
		  $xmlTwitch = $xmlDom->createElement('twitch');
		  $attrID = $xmlDom->createAttribute('id');
		  $attrID->value = $row['uid_twitch'];
		  $xmlTwitch->appendChild($attrID);
		  $xmlSpecies = $xmlDom->createElement('species');
		  $xmlText = $xmlDom->createTextNode($row['name']);
		  $xmlSpecies->appendChild($xmlText);
		  $xmlTwitch->appendChild($xmlSpecies);
		  $xmlAge = $xmlDom->createElement('age');
		  $xmlText = $xmlDom->createTextNode($row['age']);
		  $xmlAge->appendChild($xmlText);
		  $xmlTwitch->appendChild($xmlAge);
		  $xmlSex = $xmlDom->createElement('sex');
		  $xmlText = $xmlDom->createTextNode($row['sex']);
		  $xmlSex->appendChild($xmlText);
		  $xmlTwitch->appendChild($xmlSex);
		  $xmlDate = $xmlDom->createElement('date');
		  $xmlText = $xmlDom->createTextNode($row['date']);
		  $xmlDate->appendChild($xmlText);
		  $xmlTwitch->appendChild($xmlDate);
		  $xmlAddress = $xmlDom->createElement('address');
		  $xmlHouse = $xmlDom->createElement('house');
		  $xmlText = $xmlDom->createTextNode($row['house']);
		  $xmlHouse->appendChild($xmlText);
		  $xmlAddress->appendChild($xmlHouse);	
		  $xmlStreet = $xmlDom->createElement('street');
		  $xmlText = $xmlDom->createTextNode($row['street']);
		  $xmlStreet->appendChild($xmlText);
		  $xmlAddress->appendChild($xmlStreet);
		  $xmlTown = $xmlDom->createElement('town');
		  $xmlText = $xmlDom->createTextNode($row['town']);
		  $xmlTown->appendChild($xmlText);
		  $xmlAddress->appendChild($xmlTown);
		  $xmlCity = $xmlDom->createElement('city');
		  $xmlText = $xmlDom->createTextNode($row['city']);
		  $xmlCity->appendChild($xmlText);
		  $xmlAddress->appendChild($xmlCity);
		  $xmlPostcode = $xmlDom->createElement('postcode');
		  $xmlText = $xmlDom->createTextNode($row['postcode']);
		  $xmlPostcode->appendChild($xmlText);
		  $xmlAddress->appendChild($xmlPostcode);
		  $xmlLng = $xmlDom->createElement('lng');
		  $xmlText = $xmlDom->createTextNode($row['lng']);
		  $xmlLng->appendChild($xmlText);
		  $xmlAddress->appendChild($xmlLng);
		  $xmlLat = $xmlDom->createElement('lat');
		  $xmlText = $xmlDom->createTextNode($row['lat']);
		  $xmlLat->appendChild($xmlText);
		  $xmlAddress->appendChild($xmlLat);
		  $xmlDistrict = $xmlDom->createElement('district');
		  $xmlText = $xmlDom->createTextNode($row['district']);
		  $xmlDistrict->appendChild($xmlText);
		  $xmlAddress->appendChild($xmlDistrict);
		  $xmlTwitch->appendChild($xmlAddress);
		  $xmlUser = $xmlDom->createElement('user');
		  $attrID = $xmlDom->createAttribute('id');
		  $attrID->value = $row['uid_user'];
		  $xmlUser->appendChild($attrID);
		  $xmlUsername = $xmlDom->createElement('userName');
		  $xmlText = $xmlDom->createTextNode($row['username']);
		  $xmlUsername->appendChild($xmlText);
		  $xmlUser->appendChild($xmlUsername);
		  $xmlEmail = $xmlDom->createElement('email');
		  $xmlText = $xmlDom->createTextNode($row['email']);
		  $xmlEmail->appendChild($xmlText);
		  $xmlUser->appendChild($xmlEmail);
		  $xmlAct_Code = $xmlDom->createElement('act_code');
		  $xmlText = $xmlDom->createTextNode($row['act_code']);
		  $xmlAct_Code->appendChild($xmlText);
		  $xmlUser->appendChild($xmlAct_Code);
		  $xmlTwitch->appendChild($xmlUser);

		  // Get this twitch's images
		  $id = $row['uid_twitch'];
		  $imgQuery = "SELECT * FROM Image WHERE id_twitch='$id'";
		  $link = db_connect();
		  $imgResult = mysqli_query($link,$imgQuery);
		  // Add the images to the result
		  $xmlImgSet = $xmlDom->createElement('imageSet');
		  while ( $imgRow = mysqli_fetch_array($imgResult) ) 
		  {
			  $xmlImg = $xmlDom->createElement('image');
			  $attrDesc = $xmlDom->createAttribute('desc');
			  $attrDesc->value = $imgRow['description'];
			  $xmlImg->appendChild($attrDesc);
			  $attrFile = $xmlDom->createAttribute('file');
			  $attrFile->value = $imgRow['filename'] . '.jpg';
			  $xmlImg->appendChild($attrFile);
			  $xmlImgSet->appendChild($xmlImg);
		  }
		  $xmlTwitch->appendChild($xmlImgSet);
		  $xmlRoot->appendChild($xmlTwitch);
	   }
	}
	return $xmlDom->saveXML();
}

function CreateNameResult($result, $type)
{
	$xmlDom = new DOMDocument();
	$xmlDom->appendChild($xmlDom->createElement('twitches'));
	$xmlRoot = $xmlDom->documentElement;
	if ( mysqli_num_rows($result) > 0 ) {
	   while ( $row = mysqli_fetch_array($result) ) {
		  $xmlTwitch = $xmlDom->createElement('twitch');
		  if($type == 'species')
		  {
			  $xmlSpecies = $xmlDom->createElement('species');
			  $xmlText = $xmlDom->createTextNode($row['name']);
			  $xmlSpecies->appendChild($xmlText);
			  $xmlTwitch->appendChild($xmlSpecies);
		  }
		  if($type == 'location')
		  {
			  $xmlLocation = $xmlDom->createElement('location');
			  $xmlText = $xmlDom->createTextNode($row['location']);
			  $xmlLocation->appendChild($xmlText);
			  $xmlTwitch->appendChild($xmlLocation);
		  }
		  $xmlRoot->appendChild($xmlTwitch);
	   }
	}
	return $xmlDom->saveXML();
}
?>