<?php

class Student_model extends CI_Model
{
	private $table_user = 'user';
	
	public function __construct(){
		parent::__construct();
		
		$this->load->database();
	}
	
	public function hasCompleted($id_user)
	{	
		$q = "SELECT * FROM sds_user WHERE id_user = ?";
		$data = array($id_user);
		$q = $this->db->query($q, $data);
		
		if($q->num_rows() > 0)
		{
			$r = $q->result();
			if($r[0]->completed == true)
				return true;
			else
				return false;
		}
		else
			return false;
	}
	
	// Returns all students who have not completed selection
	public function notCompleted()
	{
		$q = "SELECT * FROM sds_user WHERE completed = 0";
		$q = $this->db->query($q);
		
		if($q->num_rows() > 0)
		{
			$r = $q->result_array();
			return $r;
		}
		else
			return null;
	}
	
	// All students who have chosen selected activity
	public function getCompleted($id_activity, $day)
	{
		$q = "SELECT * FROM sds_user WHERE completed = 0";
		switch($day)
		{
			case 1:
				$q = "SELECT * FROM sds_user WHERE actDay1 = ?";
				break;
			case 2:
				$q = "SELECT * FROM sds_user WHERE actDay2 = ?";
				break;
			case 3:
				$q = "SELECT * FROM sds_user WHERE actDay3 = ?";
				break;
			case 4:
				$q = "SELECT * FROM sds_user WHERE actDay4 = ?";
				break;
		}
		$data = array($activity);
		$q = $this->db->query($q,$data);
		
		if($q->num_rows() > 0)
		{
			$r = $q->result();
			return $r;
		}
		else
			return null;
	}
	
	public function setCompleted($id_user)
	{
		$q = "UPDATE sds_user SET completed = 1 WHERE id_user = ?";
		$data = array($id_user);
		$q = $this->db->query($q,$data);
	}
	
	public function checkSelections($id_user)
	{
		$q = "SELECT * FROM sds_user WHERE id_user = ?";
		$data = array($id_user);
		$q = $this->db->query($q, $data);
		
		$act1;
		$act2;
		$act3;
		$act4;
		
		if($q->num_rows() > 0)
		{
			$r = $q->result();
			
			$act1 = $r[0]->actDay1;
			$act2 = $r[0]->actDay2;
			$act3 = $r[0]->actDay3;
			$act4 = $r[0]->actDay4;
			
			if($act1 == 'Personal Study' || $act2 == 'Personal Study' || $act3 == 'Personal Study' || $act4 == 'Personal Study')
				return 1;
			else
				return 0;
		}
		else
		{
			return 0;
		}
	}
	
	public function getSelections($id_user)
	{
		$q = "SELECT * FROM sds_user WHERE id_user = ?";
		$data = array($id_user);
		$q = $this->db->query($q, $data);
		
		if($q->num_rows() > 0)
		{
			$r = $q->result_array();
			return $r[0];
		}
		else
			return null;
	}
	
	public function preSelected($id_user, $day)
	{
		$q = "SELECT * FROM sds_user WHERE id_user = ?";
		$data = array($id_user);
		$q = $this->db->query($q, $data);
		
		if($q->num_rows() > 0)
		{
			$r = $q->result();
			$activity = null;
			switch($day){
				case 1:
					$activity = $r[0]->actDay1;
					break;
				case 2:
					$activity = $r[0]->actDay2;
					break;
				case 3:
					$activity = $r[0]->actDay3;
					break;
				case 4:
					$activity = $r[0]->actDay4;
					break;
			}
			return $activity;
		}
		else
			return null;
	}
	
	public function getYear($id_user)
	{
		$q = "SELECT * FROM sds_user WHERE id_user = ?";
		$data = array($id_user);
		$q = $this->db->query($q, $data);
		
		if($q->num_rows() > 0)
		{
			$r = $q->result();
			return $r[0]->year;
		}
		else
			return 0;
	}
}