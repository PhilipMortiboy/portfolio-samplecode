<div id="content">
	<div class="padding">
	<p>Select a Self Directed Curriculum file to upload. Must be a .csv file that follows the layout specified in the user guide.</p>

	<?php echo form_open_multipart('teacher/updateActivities');?>

	<input type="file" name="userfile" size="20" />

	<br /><br />

	<input type="submit" value="Upload" />

	</form>
	<p><b>WARNING!</b> Uploading a new SDC file will delete any existing activities from the database. This will not effect student's who have already made their selections.</p>
	</div><!--Closes padding-->
</div><!--Closes content-->