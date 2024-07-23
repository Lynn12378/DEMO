<?php

header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

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
//登入
    case "login":
        $PlayerInfo = json_decode($_POST['PlayerInfo']);
        $Player_name = $PlayerInfo->Player_name;
        $Player_password = $PlayerInfo->Player_password;

        $response = array(); // 初始化 $response

        $result = Check($conn, $Player_name);
        if ($result->num_rows > 0)
        {
            while($row = $result->fetch_assoc())
            {
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
                $PlayerInfo->Player_id = $row['Player_id'];
                $PlayerInfo = GetOutfits($conn, $PlayerInfo);
                $response['PlayerInfo'] = $PlayerInfo;
            }
        }else{
            $response['status'] = "Failure"; // 名稱不存在
            $response['message'] = "Name does not exist";
        }
        header('Content-Type: application/json'); // 設置正確的 json 格式
        echo json_encode($response);
        break;
//註冊
    case "signUp":
        $PlayerInfo = json_decode($_POST['PlayerInfo']);
        $Player_name = $PlayerInfo->Player_name;
        $Player_password = $PlayerInfo->Player_password;

        $response = array(); // 初始化 $response
        $result = Check($conn, $Player_name);
        if ($result->num_rows > 0)
        {
            $response['status'] = "Failure"; // 名稱已註冊
            $response['message'] = "This name has already registered";
        }else{
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
//註冊
    case "create":
        $PlayerInfo = json_decode($_POST['PlayerInfo']);
        $Player_id = $PlayerInfo->Player_id;
        $Player_colors = $PlayerInfo->colorList;
        $Player_outfits = $PlayerInfo->outfits;

        $response = array(); // 初始化 $response

        $sql = sprintf(
            "INSERT INTO outfits (Player_id, Player_colors, Player_outfits) VALUES ('%s', '%s', '%s')",
            $Player_id,
            json_encode($Player_colors),
            json_encode($Player_outfits)
        );
        $result = $conn->query($sql);

        $response["status"] = "Success";
        $response['message'] = $sql;//"Create success";

        header('Content-Type: application/json');
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

function GetOutfits($conn, $PlayerInfo)
{
    $id = $PlayerInfo->Player_id;
    $sql = sprintf(
        "SELECT * FROM outfits WHERE Player_id= '%s' Limit 1",
        $id
    );

    $result = $conn->query($sql);
    if ($result)
    {
        $row = $result->fetch_assoc();
        $PlayerInfo->colorList = json_decode($row['Player_colors']);
        $PlayerInfo->outfits = json_decode($row['Player_outfits']);
    }
    return $PlayerInfo;
}
?>