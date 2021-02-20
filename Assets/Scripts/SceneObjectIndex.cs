using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * Creator: Nate Smith
 * Date Created: 2/10/2021
 * Description: A object index to hold relevant scene objects outside of GameManager for Services to access.
 * Likely not an ideal solution!
 */
public class SceneObjectIndex : MonoBehaviour
{
    #region Game start
    public Transform[] pawnStartPositionsRed;
    public Transform[] pawnStartPositionsBlue;
    public Pawn pawnPrefabRed;
    public Pawn pawnPrefabBlue;
    public Rigidbody ball;
    public readonly Vector3 ballInitPos = new Vector3(0f, 0f, 0f);
    #endregion

    #region Score/Time
    public Text redScoreText;
    public Text blueScoreText;
    public Text timeText;
    #endregion

    #region UI
    public Text startText;
    public Text pauseText;
    public Text gameOverText;
    public Text gameOverSubtitleText;
    #endregion

    #region Referee
    public GameObject refereePrefab;
    public Transform refereeSpawnTransform;
    #endregion

    #region Goals
    public Transform redGoal;
    public Transform blueGoal;
    #endregion
}
