<?php
include 'Connector.php';

$Action = $_GET['Action'];
$conn = connectDatabase();

if (!$conn) {
    echo "Database connection failed.";
    return;
}

switch ($Action) {
    case "getAll":
        $sql = "SELECT * FROM Player";
        $result = $conn->query($sql);

        if ($result->num_rows > 0) {
            $rows = array();
            while($row = $result->fetch_assoc()) {
                $response[] = $row; // 將資料轉換為 JSON 格式並輸出
            }
            echo json_encode($response);
        } else {
            echo json_encode(array("status" => "error", "message" => "0 results"));
        }

        break;

//註冊
    case "signUp":
        $Player_name = $_POST['Player_name'];
        $Player_password = $_POST['Player_password'];

        $response = array(); // 初始化 $response

        $result = Check($conn, $Player_name);
        if ($result->num_rows > 0)
        {
            $response['status'] = "Failure"; // 名稱已註冊
            $response['message'] = "This name has already registered";
        }
        else
        {
            $sql = sprintf(
                "INSERT INTO player (Player_name, Player_password) VALUES ('%s', '%s')",
                $Player_name,
                $Player_password
            );
        
            $result = $conn->query($sql);
            $result = Check($conn, $Player_name);

            while($row = $result->fetch_assoc())
            {
                $response['Player_id'] = $row['Player_id'];
                $response['Player_name'] = $row['Player_name'];
            }

            $response["status"] = "Success";
            $response['message'] = "Sign up success";
        }

        header('Content-Type: application/json'); // 設置正確的 json 格式
        echo json_encode($response);
        break;

//登入
    case "login":
        $Player_name = $_POST['Player_name'];
        $Player_password = $_POST['Player_password'];

        $response = array(); // 初始化 $response

        $result = Check($conn, $Player_name);
        if ($result->num_rows > 0) {
            while($row = $result->fetch_assoc()) {
                if ($row['Player_password'] == $Player_password)
                {
                    $response["status"] = "Success"; // 帳號密碼驗證成功
                    $response['message'] = "Login success";
                }
                else
                {
                    $response['status'] = "Failure"; // 密碼錯誤
                    $response['message'] = "Password incorrect";
                }
                $response['Player_id'] = $row['Player_id'];
                $response['Player_name'] = $row['Player_name'];
            }
            
        }
        else
        {
            $response['status'] = "Failure"; // 名稱不存在
            $response['message'] = "Name does not exist";
        }

        header('Content-Type: application/json'); // 設置正確的 json 格式
        echo json_encode($response);
        break;

    default:
        echo json_encode(array("status" => "error", "message" => "Unknown action."));
        break;
}

$conn->close();

function Check($conn, $Player_name)
{
    $sql = sprintf(
        "SELECT * FROM player WHERE Player_name= '%s' Limit 1",
        $Player_name
    );

    $result = $conn->query($sql);
    return $result;
}
?>