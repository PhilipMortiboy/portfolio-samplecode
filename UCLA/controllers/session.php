<?php

class Session extends CI_Controller {

	function __construct() {
		parent::__construct();
		$this->load->model('user_model');
		$this->load->library('form_validation');
		$this->load->library('session');
		$this->load->helper('date');
		$this->load->helper('url');
	}
	
	// If user access this controller directly, redirect to appropriate function
	function index(){
		$this->load->model('user_model');
		if($this->user_model->isLoggedIn())
		{
			$this->dashboard();
		} else {
			$this->login();
		}
	}

	// Login function
	function login($error=NULL){
		$data['username'] = NULL;
		$data['error'] = FALSE;
		$this->load->model('user_model');
		
		// If logged in, redirected to dashboard function
		if($this->user_model->isLoggedIn()){
			$this->dashboard();
		}
		else
		{
			$this->load->library('form_validation');
			$this->form_validation->set_error_delimiters('<span class="error_form">','</span>');
			
			// Username and password required
			$this->form_validation->set_rules('username', ' ', 'required');
			$this->form_validation->set_rules('password', ' ', 'required');
			
			// Display form
			if(!$this->form_validation->run()){
				$data['error'] = '';
				$this->load->view('components/head');
				$this->load->view('components/banner');
			   	$this->load->view('login', $data);
				$this->load->view('components/footer');
			
			// If form submitted, check ok
			} else {
				$username = $this->input->post('username');
				$password = $this->input->post('password');
				$validCredentials = $this->user_model->validCredentials($username,$password);
				// Login ok
				if($validCredentials==1)
				{
					$this->dashboard();
				}
				// Wrong username/password
				elseif($validCredentials==0)
				{
					$data['error'] = 'Invalid Email/Password';
					$this->load->view('components/head');
					$this->load->view('components/banner');
					$this->load->view('login', $data);
					$this->load->view('components/footer');
				}
			}
		}
	}
	
	function adminLogin($error=NULL){
		$data['username'] = NULL;
		$data['error'] = FALSE;
		$this->load->model('user_model');
		
		// If logged in, redirected to dashboard function
		if($this->user_model->isLoggedIn()){
			$this->dashboard();
		}
		else
		{
			$this->load->library('form_validation');
			$this->form_validation->set_error_delimiters('<span class="error_form">','</span>');
			
			// Username and password required
			$this->form_validation->set_rules('username', ' ', 'required');
			$this->form_validation->set_rules('password', ' ', 'required');
			
			// Display form
			if(!$this->form_validation->run()){
			   	$data['error'] = '';
				$this->load->view('components/head');
				$this->load->view('components/banner');
			   	$this->load->view('adminLogin', $data);
				$this->load->view('components/footer');
			
			// If form submitted, check ok
			} else {
				$username = $this->input->post('username');
				$password = $this->input->post('password');
				$validCredentials = $this->user_model->adminValidCredentials($username,$password);
				// Login ok
				if($validCredentials==1)
				{
					$this->dashboard();
				}
				// Wrong username/password
				elseif($validCredentials==0)
				{
					$data['error'] = 'Invalid Email/Password';
					$this->load->view('components/head');
					$this->load->view('components/banner');
					$this->load->view('adminLogin', $data);
					$this->load->view('components/footer');
				}
			}
		}
	}
	
	// Function to direct user to correct page post login
	function dashboard(){
		$this->load->model('user_model');
		// 3 is admin account
		if($this->user_model->isLoggedIn() && $this->session->userdata('id_account') == 3) {
		// Redirect to teacher controller
			redirect('teacher');
		}
		elseif($this->user_model->isLoggedIn() && $this->session->userdata('id_account') != 3) {
			// Redirect to student controller
			redirect('student/start');
		}
	}
	
	// Logout function
	function logout(){
		// Log user out and redirect to home page
		$this->load->model('user_model');
		if($this->user_model->isLoggedIn()){
			$this->user_model->logout();
			redirect('welcome/');
		}
		// If user tries logging out when not logged in, redirect to login page
		else{ 
			$this->login();
		}
	}
}