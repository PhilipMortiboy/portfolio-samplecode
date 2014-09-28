<div id="content">
	<div class="padding">
	<p>Select a Students table file to upload. Must be a .csv file that follows the layout specified in the user guide.</p>

	<?php echo form_open_multipart('teacher/updateStudents');?>

	<input type="file" name="userfile" size="20" />

	<br /><br />

	<input type="submit" value="Upload" />

	</form>
	<p><b>WARNING!</b> Uploading a new students file will delete any existing student records, <b>including student's course choices.</b></p>
	</div><!--Closes padding-->
</div><!--Closes content-->