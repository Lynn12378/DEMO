<?php
include 'Connector.php';

$conn = connectDatabase();

if (!$conn) 
{
    echo "Database connection failed.";
    return;
}

$PlayerOutputData = json_decode($_POST['PlayerOutputData']);

if (isset($PlayerOutputData->playerId)) {
    $playerId = (int) $PlayerOutputData->playerId;
    $killNo = (int) $PlayerOutputData->killNo;
    $deathNo = (int) $PlayerOutputData->deathNo;
    $surviveTime = (float) $PlayerOutputData->surviveTime;
    $collisionNo = (int) $PlayerOutputData->collisionNo;
    $bulletCollision = (int) $PlayerOutputData->bulletCollision;
    $bulletCollisionOnLiving = (int) $PlayerOutputData->bulletCollisionOnLiving;
    $remainHP = json_encode($PlayerOutputData->remainHP);
    $remainBullet = json_encode($PlayerOutputData->remainBullet);
    $totalVoiceDetectionDuration = (float) $PlayerOutputData->totalVoiceDetectionDuration;
    $organizeNo = (int) $PlayerOutputData->organizeNo;
    $fullNo = (int) $PlayerOutputData->fullNo;
    $placeholderNo = (int) $PlayerOutputData->placeholderNo;
    $rankNo = (int) $PlayerOutputData->rankNo;
    $giftNo = (int) $PlayerOutputData->giftNo;
    $createTeamNo = (int) $PlayerOutputData->createTeamNo;
    $joinTeamNo = (int) $PlayerOutputData->joinTeamNo;
    $quitTeamNo = (int) $PlayerOutputData->quitTeamNo;
    $repairQuantity = (int) $PlayerOutputData->repairQuantity;
    $restartNo = (int) $PlayerOutputData->restartNo;
    $usePlaceholderNo = (int) $PlayerOutputData->usePlaceholderNo;
    $feedNo = (int) $PlayerOutputData->feedNo;

    $sql = "INSERT INTO output_data (playerId, killNo, deathNo, surviveTime, collisionNo, bulletCollision, bulletCollisionOnLiving, 
                                    remainHP, remainBullet, totalVoiceDetectionDuration, organizeNo, fullNo, placeholderNo, rankNo, 
                                    giftNo, createTeamNo, joinTeamNo, quitTeamNo, repairQuantity, restartNo, usePlaceholderNo, feedNo) 
            VALUES ($playerId, $killNo, $deathNo, $surviveTime, $collisionNo, $bulletCollision, $bulletCollisionOnLiving, 
                    '$remainHP', '$remainBullet', $totalVoiceDetectionDuration, $organizeNo, $fullNo, $placeholderNo, $rankNo, 
                    $giftNo, $createTeamNo, $joinTeamNo, $quitTeamNo, $repairQuantity, $restartNo, $usePlaceholderNo, $feedNo)";}

$response = array(); // Initialize response array

if ($conn->query($sql) === TRUE) 
{
    $response['status'] = "Success";
    $response['message'] = "PlayerOutputData inserted successfully.";
} else 
{
    $response['status'] = "Failure";
    $response['message'] = "Error: " . $sql . "<br>" . $conn->error;
}

header('Content-Type: application/json');
echo json_encode($response);

$conn->close();
?>
