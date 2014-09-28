<?php if ( ! defined('BASEPATH')) exit('No direct script access allowed');

class Welcome extends CI_Controller {

	public function index()
	{
		// Create database if one does not exist already
		$this->load->model('database_model');
		$this->database_model->checkforDb();
		
		$this->load->model('user_model');
		if($this->user_model->isLoggedIn())
		{
			// Redirect to user's home page
			redirect('session/dashboard');
		} else {
			// Redirect to login
			redirect('session/login');
		}
	}
}