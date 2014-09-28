<?php

class User_model extends CI_Model
{
	private $table_user = 'sds_user';
	
	public function __construct(){
		parent::__construct();
		
		$this->load->database();
	}
	
	public function changePassword($email, $password, $new_password)
	{
		$this->load->library('encrypt');
		$password_sha1 = $this->encrypt->sha1($password);
		
		$q = "SELECT * FROM sds_user WHERE username = ? AND password = ?";
		
		$data = array($email, $password_sha1);
		$q = $this->db->query($q, $data);
		
		if($q->num_rows() == 1)
		{
			$new_password_sha1 = $this->encrypt->sha1($new_password);
			$this->db->where(array('username' => $email, 'password' => $password_sha1));
			return $this->db->update($this->table_user, array('password' => $new_password_sha1));
		}
		else
			return false;
	}

	public function validCredentials($username,$password){
		$this->load->library('encrypt');
		
		$password = $this->encrypt->sha1($password);
		
		$q = "SELECT * FROM sds_user WHERE username = ? AND password = ?";
		
		$data = array($username,$password);
		$q = $this->db->query($q,$data);
		
		if($q->num_rows() > 0)
		{
			$r = $q->result();
			
				$session_data = array('username' => $r[0]->username,'logged_in' => true, 'id_account' => $r[0]->id_account, 'id_user' => $r[0]->id_user);
				$this->session->set_userdata($session_data);
				return true;
				
		}
		else
			return false;
	}
	
	public function adminValidCredentials($username,$password){
		$this->load->library('encrypt');
		
		$password = $this->encrypt->sha1($password);
		
		$q = "SELECT * FROM sds_admin WHERE username = ? AND password = ?";
		
		$data = array($username,$password);
		$q = $this->db->query($q,$data);
		
		if($q->num_rows() > 0)
		{
			$r = $q->result();
			
				$session_data = array('username' => $r[0]->username,'logged_in' => true, 'id_account' => 3, 'id_user' => $r[0]->id_admin);
				$this->session->set_userdata($session_data);
				return true;
				
		}
		else
			return false;
	}
	
	public function isLoggedIn(){
			if($this->session->userdata('logged_in'))
			{ return true; } else { return false; }
	}
	
	public function logout(){
		if($this->session->userdata('logged_in'))
		{ return $this->session->sess_destroy(); }
		else { return false; }
	}
	
	public function userID(){
		$id = $this->session->userdata('id_user');
		return $id;
	}
	
	// Using email - for searches
	public function getUserDetails($email)
	{
		$query = $this->db->get_where($this->table_user, array('email' => $email));
						
		if($query->num_rows() > 0){
			$rows = $query->result();
			return $rows[0];
		}
		else return NULL;
	}
	
	// Using id
	public function getUser($id)
	{
		$query = $this->db->get_where($this->table_user ,array('id_user'=>$id));
	
		if($query->num_rows()>0)
		{
			$rows = $query->result();
			return $rows[0];
		}
	}
	
	public function searchFirstName($firstname)
	{
		$query = $this->db->get_where($this->table_user ,array('fName'=>$firstname));
	
		if($query->num_rows()>0)
		{
			$rows = $query->result();
			return $rows[0];
		}
	}
	
	public function searchSecondName($secondname)
	{
		$query = $this->db->get_where($this->table_user ,array('lName'=>$secondname));
	
		if($query->num_rows()>0)
		{
			$rows = $query->result();
			return $rows[0];
		}
	}
	
	
}