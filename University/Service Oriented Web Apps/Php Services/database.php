<?php
	function db_connect()
	{
		// Database settings
		$host = '';
		$user = '';
		$passwd = '';
		$dbname = '';

		// Establish db connection.
		$link = mysqli_connect($host, $user, $passwd) or die('Failed to connect to MySQL server. ' . mysqli_connect_error() .'<br />');
		mysqli_select_db($link, $dbname) or die('Failed to connect to the database. ' . mysqli_connect_error());

		return $link;
	}
?>