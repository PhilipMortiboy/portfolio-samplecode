<div id="content">
	<div class="padding">
	<h1><?php echo $day; ?></h1>
	<p>You have selected <b><?php echo $selection; ?></b> for your acitity on <b><?php echo $day; ?></b>.
	<p>If you're happy with this, click the done button, or if not click the back button to change your choice.</p>
	
	<a href="<?php echo base_url('student/edit/'.$dayNum.''); ?>">
		<input type="submit" name="submit" value="Back" />
	</a>
	<a href="<?php echo base_url('student/checkCompletion'); ?>">
		<input type="submit" name="submit" value="Done" />
	</a>
	</div><!--Closes padding-->
</div><!--Closes content-->