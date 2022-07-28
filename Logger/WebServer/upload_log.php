<?php
$desc = $_POST["desc"];
$tmp = $_FILES["logfile"]["tmp_name"];
$savePath = "upload/" . $_FILES["logfile"]["name"];

if (file_exists($savePath))
{
	// 文件已存在，删除它
	unlink($savePath);
}

move_uploaded_file($tmp, $savePath);
echo "文件上传成功";
?>