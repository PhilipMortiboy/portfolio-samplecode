<div id="content">
	<div class="padding">
<p>Please enter your login details to access the teacher area:</p>
<font color="ff0000"><?php echo $error; ?></font>

<?php echo form_open('session/adminLogin'); ?>

	<label for="username"><strong>Username: </strong></label> 
	<input type="input" name="username" value=""/><br/>
	<p></p>
	
	<label for="password"><strong>Password: </strong></label> 
	<input type="password" name="password" value=""/><br/>
	<p></p>
	
	<input type="submit" name="submit" value="Login" /> 

	</div><!--Closes padding-->
</div><!--Closes content-->