<?php if ( ! defined('BASEPATH')) exit('No direct script access allowed');

class Teacher extends CI_Controller {

	public function __construct()
	{
		parent::__construct();
		
		$this->load->helper(array('form', 'url'));
		
		$this->load->model('user_model');
		$this->load->model('database_model');
		// Redirect login if not logged in or no admin rights
		if(!$this->user_model->isLoggedIn() && $this->session->userdata('id_account') != 3){
			redirect('session/adminLogin');
		}
		if($this->session->userdata('id_account') != 3)
			redirect('session/adminLogin');
	}
	
	public function index()
	{	
		$data = $this->database_model->isLocked();
		
		$this->load->view('components/head');
		$this->load->view('components/banner');
		$this->load->view('components/admin_nav');
		$this->load->view('admin', $data);
		$this->load->view('components/footer');
	}
	
	// Loads update students page
	public function updateStudentsPage()
	{
		$this->load->view('components/head');
		$this->load->view('components/banner');
		$this->load->view('components/admin_nav');
		$this->load->view('update_students');
		$this->load->view('components/footer');
	}
	
	// Upload student excel file
	public function updateStudents()
	{
		$config['upload_path'] = './uploads/'; 
		$config['allowed_types'] = '*'; 
		$config['max_size']	= '2048'; //in KB
		$config['file_name'] = 'students.csv';

		$this->load->library('upload', $config);

		if ( ! $this->upload->do_upload())
		{
			$error = array('error' => $this->upload->display_errors());

			$this->load->view('upload_failed', $error); 
		}
		else
		{
			$data = $this->upload->data();
			$error = $this->database_model->processStudentUpdate($data);
			
			if($error == true){
				$this->load->view('process_failed');
			}else{
				$data['type'] = 'upStu';
				$this->successPage($data);
			}
		}
	}
	
	// Loads update activities page
	public function updateActivitiesPage()
	{
		$this->load->view('components/head');
		$this->load->view('components/banner');
		$this->load->view('components/admin_nav');
		$this->load->view('update_activities');
		$this->load->view('components/footer');
	}
	
	// Upload activities excel file
	public function updateActivities()
	{
		$config['upload_path'] = './uploads/'; 
		$config['allowed_types'] = '*'; 
		$config['max_size']	= '2048'; //in KB

		$this->load->library('upload', $config);

		if ( ! $this->upload->do_upload())
		{
			$error = array('error' => $this->upload->display_errors());

			$this->load->view('upload_failed', $error); 
		}
		else
		{
			$data = $this->upload->data();
			$error = $this->database_model->processActivitiesUpdate($data);
			
			if($error == true){
				$this->load->view('process_failed');
			}else{
				$data['type'] = 'upAct';
				$this->successPage($data);
			}
		}
	}
	
	// Download current database as csv
	public function downloadDB()
	{
		// Create the csv file
		$error = $this->database_model->createCSV('db');
		if($error == true){
			$this->load->view('process_failed');
		}else{
			$this->load->helper('download');
			$data = file_get_contents("/myDb.csv"); 
			$name = 'myDb.csv';
			force_download($name, $data);
			
			$sucessData['type'] = 'downloadDb';
			$this->successPage($successData);
		}
	}
	
	// Download list of students as csv
	public function downloadStudentList()
	{
		// Create the csv file
		$error = $this->database_model->createCSV('student');
		if($error == true){
			$this->load->view('process_failed');
		}else{
			$this->load->helper('download');
			$data = file_get_contents("/students.csv"); 
			$name = 'myStu.csv';
			force_download($name, $data);
			
			$sucessData['type'] = 'downloadStu';
			$this->successPage($successData);
		}
	}
	
	public function successPage($data)
	{
		$this->load->view('components/head');
		$this->load->view('components/banner');
		$this->load->view('components/admin_nav');
		$this->load->view('upload_success', $data);
		$this->load->view('components/footer');
	}
	
	// List of students who have not completed selection
	public function notCompleted()
	{
		$this->load->model('student_model');
		$data['student'] = $this->student_model->notCompleted();
		
		$this->load->view('components/head');
		$this->load->view('components/banner');
		$this->load->view('components/admin_nav');
		$this->load->view('not_completed', $data);
		$this->load->view('components/footer');
	}
	
	public function viewCompletionHome()
	{
		$data = $this->database_model->getAllActivites();
		
		$this->load->view('components/head');
		$this->load->view('components/banner');
		$this->load->view('components/admin_nav');
		$this->load->view('completions_home', $data);
		$this->load->view('components/footer');
	}
	
	public function viewCompletion($id_activity, $day)
	{
		$data = $this->database_model->getCompleted($id_activity, $day);
		
		$this->load->view('components/head');
		$this->load->view('components/banner');
		$this->load->view('components/admin_nav');
		$this->load->view('view_completion', $data);
		$this->load->view('components/footer');
	}
}