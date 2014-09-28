<?php if ( ! defined('BASEPATH')) exit('No direct script access allowed');

class Student extends CI_Controller {

	public function __construct()
	{
		parent::__construct();
		
		$this->load->helper('url');
		$this->load->model('database_model');
		$this->load->model('student_model');
		$this->load->model('user_model');
		
		// Redirect login if not logged in 
		if(!$this->user_model->isLoggedIn()){
			redirect('session/login');
		}
	}
	
	public function index()
	{
		$completed = $this->student_model->hasCompleted($this->user_model->userID());
		if($completed == 1){
			$this->completed();
		}else{
			$this->start();
		}
	}
	
	public function start()
	{
		$completed = $this->student_model->hasCompleted($this->user_model->userID());
		if($completed == 1)
		{
			$this->completed();
		}else{
		
			$data['name'] = $this->session->userdata('username');
			
			$this->load->view('components/head');
			$this->load->view('components/banner');
			$this->load->view('start', $data);
			$this->load->view('components/footer');
		}
	}
	
	public function day($number)
	{
		$thisDay = $number;
		$id_user = $this->user_model->userID();
		
		$this->load->helper('form');
		$this->load->library('form_validation');
		$this->form_validation->set_rules('selection', 'Selection', 'required');
		$data['day'] = $this->database_model->getDay($thisDay);
		$data['dayNext'] = $thisDay + 1;
		$data['dayNum'] = $thisDay;
		
		if ($this->form_validation->run() == FALSE)
		{
			$year = $this->student_model->getYear($this->user_model->userID());
			$data['pre_selected'] = $this->student_model->preSelected($id_user, $thisDay);
			$data['activity'] = $this->database_model->getActivities($thisDay, $year);

			$this->load->view('components/head');
			$this->load->view('components/banner');
			$this->load->view('day', $data);
			$this->load->view('components/footer');
		}
		else
		{
			$this->database_model->saveSelection($id_user, $thisDay);
			$data['selection'] = $this->input->post('selection');
			// If last day
			if($thisDay == 4)
			{
				// Check selections are ok and move to summary
				$completed = $this->student_model->checkSelections($id_user);
				$this->summary($completed);
			}
			else
			{
				$this->load->view('components/head');
				$this->load->view('components/banner');
				$this->load->view('completed_day', $data);
				$this->load->view('components/footer');
			}
		}
	}
	
	public function edit($number)
	{
		$thisDay = $number;
		$id_user = $this->user_model->userID();
		
		$this->load->helper('form');
		$this->load->library('form_validation');
		$this->form_validation->set_rules('selection', 'Selection', 'required');
		$data['day'] = $this->database_model->getDay($thisDay);
		$data['dayNext'] = $thisDay + 1;
		$data['dayNum'] = $thisDay;
		
		if ($this->form_validation->run() == FALSE)
		{
			$year = $this->student_model->getYear($this->user_model->userID());
			$data['pre_selected'] = null;
			$data['activity'] = $this->database_model->getActivities($thisDay, $year);

			$this->load->view('components/head');
			$this->load->view('components/banner');
			$this->load->view('edit', $data); 
			$this->load->view('components/footer');
		}
		else
		{
			$this->database_model->saveSelection($id_user, $thisDay);
			$data['selection'] = $this->input->post('selection');
			
			$this->load->view('components/head');
			$this->load->view('components/banner');
			$this->load->view('completed_edit', $data);
			$this->load->view('components/footer');
		}
	}
	
	public function checkCompletion()
	{
		$id_user = $this->user_model->userID();
		$completed = $this->student_model->checkSelections($id_user);
		$this->summary($completed);
	}
	
	public function summary($complete)
	{
		$data['name'] = $this->session->userdata('username');
		$data['complete'] = $complete;
		$data['summary'] = $this->student_model->getSelections($this->user_model->userID());
			
		$this->load->view('components/head');
		$this->load->view('components/banner');
		$this->load->view('summary', $data);
		$this->load->view('components/footer');
	}
	
	public function finish()
	{
		$id_user = $this->user_model->userID();
		$completed = $this->student_model->checkSelections($id_user);
		// Only access once selections have been completed
		if($completed == 1)
		{
			$this->student_model->setCompleted($id_user);
			$this->load->view('components/head');
			$this->load->view('components/banner');
			$this->load->view('finished');
			$this->load->view('components/footer');
		}
		else
		{
			$this->start();
		}
	}
	
	// Student already selected options
	public function completed()
	{
		$data['summary'] = $this->student_model->getSelections($this->user_model->userID());
		$data['name'] = $this->session->userdata('username');
		
		$this->load->view('components/head');
		$this->load->view('components/banner');
		$this->load->view('completed', $data);
		$this->load->view('components/footer');
	}
}