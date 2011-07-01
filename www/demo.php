<?php

	// Copyright (C) 2008 Teztech, Inc.
	// All Rights Reserved

	$ErrorMessage = '';
?>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>
File Conversion Server Demo
</title>
</head>
<body>

<h1>File Conversion Server Demo</h1>

<form name="Demo" method="POST" enctype="multipart/form-data" action="/demo.pdf">

	<?php if($ErrorMessage): ?>
	<p><font size="5" color="#008000"><?php echo $ErrorMessage; ?></font></p>
	<?php endif; ?>
	
	<table>
		<tr>
			<td><b>Username:</b></td>
			<td><input name="Username" value=""></input></td>
		</tr>
		<tr>
			<td><b>Password:</b></td>
			<td><input name="Password" value=""></input></td>
		</tr>
		<tr>
			<td><b>Input File:</b></td>
			<td><input type="file" name="InputFile"></input> </td>
		</tr>
	</table>
	
	<input type="submit" value="Submit"></input> 
	
</html>
