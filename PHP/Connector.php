<?php

header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

function connectDatabase()
{
    // 連接mysql
    $conn = new mysqli("mis-demo-db.clq0ou6gejev.ap-southeast-1.rds.amazonaws.com", "admin_user", "ncumis2024", "mis_demo");

    // 檢查連線
    if ($conn->connect_error)
    {
        error_log("Database connection failed: " . $conn->connect_error);
        return null;
    }

    return $conn;
}
?>
