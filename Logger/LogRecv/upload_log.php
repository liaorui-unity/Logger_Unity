<?php




// ��ȡ��ʱ�ļ�·��

$tmp = $_FILES["logfile"]["tmp_name"];




$folder=$_POST["folder"];//���ݱ��ֶν����ļ��е��ַ�����Ϣ
$savefolder = "upload/" .$folder;

   if(!file_exists($savefolder ))
     mkdir($savefolder,0777);


// �ļ�����λ��

$savePath = "upload/" .$folder."/". $_FILES["logfile"]["name"];



// �ж��ļ��Ƿ��Ѵ���

if (file_exists($savePath))

{

	
// �ļ��Ѵ��ڣ�ɾ����
	
    unlink($savePath);

}


// �����ļ���savePath��·����

move_uploaded_file($tmp, $savePath);


echo "�ļ��ϴ��ɹ�";
?>
