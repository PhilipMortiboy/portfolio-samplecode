<?php

class Database_model extends CI_Model
{
	
	public function __construct(){
		parent::__construct();
		
		$this->load->database();
		
	}
	
	// Create a new database if one does not exist
	public function checkforDb()
	{
		if (!$this->db->table_exists('sds_admin'))
		{
			$this->load->library('encrypt');
			
			$q = "CREATE TABLE sds_admin 
					(
					id_admin INT NOT NULL AUTO_INCREMENT, 
					PRIMARY KEY(id_admin),
					username VARCHAR(255),
					password VARCHAR(255),
					id_account INT
					)";
			$this->db->query($q);
			
			// Insert default admin user
			$password = $this->encrypt->sha1('chocolate007');
			$q = "INSERT INTO sds_admin (username, password, id_account) VALUES (?, ?, ?)";
			$data = array(
						'admin',
						$password,
						3,
					);
			$this->db->query($q, $data);
		}
		if (!$this->db->table_exists('sds_activity'))
		{
			$q = "CREATE TABLE sds_activity 
					(
					id_activity INT NOT NULL AUTO_INCREMENT, 
					PRIMARY KEY(id_activity),
					name VARCHAR(255),
					maxNum INT,
					day VARCHAR(10),
					year VARCHAR(12),
					active VARCHAR(10)
					)";
			$this->db->query($q);
		}
		if (!$this->db->table_exists('sds_user'))
		{
			$q = "CREATE TABLE sds_user
					(
					id_user INT NOT NULL AUTO_INCREMENT, 
					PRIMARY KEY(id_user),
					username VARCHAR(255),
					password VARCHAR(255),
					fName VARCHAR(150),
					lName VARCHAR(150),
					DoB VARCHAR(12),
					year VARCHAR(50),
					form VARCHAR(20),
					actDay1 VARCHAR(255),
					actDay2 VARCHAR(255),
					actDay3 VARCHAR(255),
					actDay4 VARCHAR(255),
					id_account INT,
					completed INT
					)";
			$this->db->query($q);
		}
	}
	// Add csv contents to db
	public function processStudentUpdate($data)
	{
		$this->load->library('encrypt');
		
		// Get data from csv file
		$filename = $data['full_path'];
		$delimiter = ',';
		if(!file_exists($filename) || !is_readable($filename))
			return FALSE;
			
		$header = NULL;
		$data = array();
		if (($handle = fopen($filename, 'r')) !== FALSE)
		{
			while (($row = fgetcsv($handle, 1000, $delimiter)) !== FALSE)
			{
				if(!$header)
					$header = $row;
				else
				$data[] = array_combine($header, $row);
			}
			fclose($handle);
		}
		
		// Remove all existing records
		$q = "DELETE FROM sds_user";
		$this->db->query($q);
		
		// Update user table
		foreach($data as $myrow)
		{
			$username = ''.$myrow['First Name'].' '.$myrow['Last Name'].'';
			$password = $this->encrypt->sha1($myrow['Form']);
			
			$q = "INSERT INTO sds_user (username, password, fName, lName, DoB, year, form, actDay1, actDay2, actDay3, actday4, id_account, completed) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
			$data = array(
						$username,
						$password,
						$myrow['First Name'],
						$myrow['Last Name'],
						$myrow['D.O.B'],
						$myrow['Year Group'],
						$myrow['Form'],
						$myrow['Monday'],
						$myrow['Wednesday'],
						$myrow['Thursday'],
						$myrow['Friday'],
						1,
						0,
					);
			$this->db->query($q, $data);
		}
	}
	
	public function processActivitiesUpdate($data)
	{
		// Get data from csv file
		$filename = $data['full_path'];
		$delimiter = ',';
		if(!file_exists($filename) || !is_readable($filename))
			return FALSE;
			
		$header = NULL;
		$data = array();
		if (($handle = fopen($filename, 'r')) !== FALSE)
		{
			while (($row = fgetcsv($handle, 1000, $delimiter)) !== FALSE)
			{
				if(!$header)
					$header = $row;
				else
				$data[] = array_combine($header, $row);
			}
			fclose($handle);
		}
		
		// Remove all existing records
		$q = "DELETE FROM sds_activity";
		$this->db->query($q);
		
		// Update activities table
		foreach($data as $myrow)
		{
			$dayNum = $this->getDayNum($myrow['Day']); 
			
			$q = "INSERT INTO sds_activity (name, maxNum, day, year, active) VALUES (?, ?, ?, ?, ?)";
			$data = array(
						$myrow['Self Directed Session'],
						$myrow['No. Of Students'],
						$dayNum,
						$myrow['Year group'],
						$myrow['Available'],
					);
			$this->db->query($q, $data);
		}
	}
	
	public function getDay($dayNum)
	{
		$dayName;
		switch($dayNum){
			case 1:
				$dayName = 'Monday';
				break;
			case 2:
				$dayName = 'Wednesday';
				break;
			case 3:
				$dayName = 'Thursday';
				break;
			case 4:
				$dayName = 'Friday';
				break;
			default:
				$dayName = 'blank';
				break;
		}
		return $dayName;
	}
	
	public function getDayNum($day)
	{
		$dayNum;
		switch($day){
			case 'Monday':
				$dayNum = 1;
				break;
			case 'Wednesday':
				$dayNum = 2;
				break;
			case 'Thursday':
				$dayNum = 3;
				break;
			case 'Friday':
				$dayNum = 4;
				break;
			default:
				$dayNum = 2;
				break;
		}
		return $dayNum;
	}
	
	public function getActivities($day, $year)
	{
		$q = "SELECT * FROM sds_activity WHERE day = ? AND year = ?";
		$data = array($day,$year);
		$q = $this->db->query($q,$data);
		
		$returnData = array();
		if($q->num_rows() > 0)
		{
			$activties = $q->result_array();
			foreach($activties as $activity)
			{
				$q2;
				switch($day)
				{
					case 1:
						$q2 = "SELECT * FROM sds_user WHERE actDay1 = ?";
						break;
					case 2:
						$q2 = "SELECT * FROM sds_user WHERE actDay2 = ?";
						break;
					case 3:
						$q2 = "SELECT * FROM sds_user WHERE actDay3 = ?";
						break;
					case 4:
						$q2 = "SELECT * FROM sds_user WHERE actDay4 = ?";
						break;
				}
				$actData = array($activity['name']);
				$q2 = $this->db->query($q2,$actData);
				if($q2->num_rows() < $activity['maxNum'])
				{
					$returnData[] = $activity;
				}
			}
			return $returnData;
		}
		else
			return null;
	}
	
	public function saveSelection($id_user, $day)
	{
		$q; //query string
		switch($day)
		{
			case 1:
				$q = "UPDATE sds_user SET actDay1 = ? WHERE id_user = ?";
				break;
			case 2:
				$q = "UPDATE sds_user SET actDay2 = ? WHERE id_user = ?";
				break;
			case 3:
				$q = "UPDATE sds_user SET actDay3 = ? WHERE id_user = ?";
				break;
			case 4:
				$q = "UPDATE sds_user SET actDay4 = ? WHERE id_user = ?";
				break;
		}
		$data = array(
			$this->input->post('selection'),
			$id_user
		);
		$q = $this->db->query($q,$data);
	}
	
	// Check if database is locked from editing - TODO: not yet implemented
	public function isLocked()
	{
		return null;
	}
}
?>