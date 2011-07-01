<?php
	if ($argc != 3)
	{
		echo "ConvertToPdf.php - Uses open office to convert a file to PDF.";
		echo "  Usage: ConvertToPdf.php srcFile destFile";
		exit(-1);
	}
	
	set_error_handler("exception_error_handler");
	
	$serviceManager = new COM("com.sun.star.ServiceManager"); 
	$desktop = $serviceManager->createInstance("com.sun.star.frame.Desktop");

	$hiddenProperty = $serviceManager->Bridge_GetStruct("com.sun.star.beans.PropertyValue");
	$hiddenProperty->Name  = "Hidden";
	$hiddenProperty->Value = TRUE;
	$docProperties = array($hiddenProperty);
	$doc = $desktop->loadComponentFromURL(ConvertPath($argv[1]), "_blank", 0, $docProperties); 
	
	$filterNameProperty = $serviceManager->Bridge_GetStruct("com.sun.star.beans.PropertyValue");
	$filterNameProperty->Name  = "FilterName";
	$filterNameProperty->Value = "writer_pdf_Export";
	$storeProperties = array($filterNameProperty);
	$doc->storeToURL(ConvertPath($argv[2]), $storeProperties);

	usleep(1); // Workaround for weird xlsx->pdf problem 
	$doc->dispose();
	
	exit(0);
	
function ConvertPath($file)
{
	$file = str_replace("\\", "/", $file);
	return "file:///" . $file;
}
	
function exception_error_handler($errno, $errstr, $errfile, $errline ) 
{
    throw new ErrorException($errstr, 0, $errno, $errfile, $errline);
}	
	
?>
