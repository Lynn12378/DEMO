<?php
function connectDatabase()
{
    // 連接mysql
    $conn = new mysqli("localhost", "Admin", "1234", "demo");

    // 檢查連線
    if ($conn->connect_error)
    {
        error_log("Database connection failed: " . $conn->connect_error);
        return null;
    }

    return $conn;
}
?>
