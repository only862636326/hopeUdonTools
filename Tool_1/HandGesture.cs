
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class HandGesture : UdonSharpBehaviour
    {
        private void Start()
        {
            local_player_height = GetLocalAvatarHeight();
            gestureHistory = new int[maxGestureHistory];
        }

        private void Update()
        {                               
            UpdateGestureControl();
        }

        public string s_Gesture;

        [SerializeField] private int maxGestureHistory = 20;
        private int[] gestureHistory;
        private int gestureHistoryCount = 1;

        private string GestureToLabel(int gesture)
        {
            switch (gesture)
            {
                case GESTURE_HAND_CASH_UP:         return " ↑";
                case GESTURE_HAND_CASH_DOWN:       return " ↓";
                case GESTURE_HAND_CASH_LEFT:       return " ←";
                case GESTURE_HAND_CASH_RIGHT:      return " →";
                case GESTURE_HAND_CASH_FORWARD:    return " F";
                case GESTURE_HAND_CASH_BACKWARD:    return " B";
                case GESTURE_HAND_CASH_UP_LEFT:    return " ↖";
                case GESTURE_HAND_CASH_UP_RIGHT:   return " ↗";
                case GESTURE_HAND_CASH_DOWN_LEFT:  return " ↙";
                case GESTURE_HAND_CASH_DOWN_RIGHT: return " ↘";
                default:                            return " ?";
            }
        }

        private void PushGestureToHistory(int gesture)
        {
            // skip if same as last detected gesture
            if (gestureHistoryCount <= 0)
                return;

            //if (gestureHistory[gestureHistoryCount - 1] == gesture)
            //    return;

            // shift left if buffer full
            if (gestureHistoryCount >= maxGestureHistory)
            {
                for (int i = 0; i < maxGestureHistory - 1; i++)
                    gestureHistory[i] = gestureHistory[i + 1];
                gestureHistoryCount = maxGestureHistory - 1;
            }
            gestureHistory[gestureHistoryCount] = gesture;
            gestureHistoryCount++;

            // build display string
            string display = "";
            for (int i = 0; i < gestureHistoryCount; i++)
            {
                if (i > 0) display += " |";
                display += GestureToLabel(gestureHistory[i]);
            }
            LogMsg(display);
        }

        //-----------Gesture tracking---------------
        const int HAND_LEFT = 0;
        const int HAND_RIGHT = 1;

        private Vector3 DIR_UP_LEFT = (Vector3.up + Vector3.left).normalized;
        private Vector3 DIR_UP_RIGHT = (Vector3.up + Vector3.right).normalized;
        private Vector3 DIR_DOWN_LEFT = (Vector3.down + Vector3.left).normalized;
        private Vector3 DIR_DOWN_RIGHT = (Vector3.down + Vector3.right).normalized;

        public float MAX_ANGLE = 15.0f;

        public float REQUIRED_OPENING_DISTANCE = 0.4f;
        public float REQUIRED_CLOSING_DISTANCE = 0.4f;

        public float REQUIRED_MOVE_SPEED = 0.0f;

        private VRCPlayerApi.TrackingData rightHand;
        private VRCPlayerApi.TrackingData prev_rightHand;
        private VRCPlayerApi.TrackingData leftHand;
        private VRCPlayerApi.TrackingData prev_leftHand;
        private VRCPlayerApi.TrackingData head;
        private VRCPlayerApi.TrackingData prev_head;

        private float speed_cal_time = 0;
        private float prev_speed_cal_time = 0;
        private float time_delta = 0;

        private float local_player_height;
        [HideInInspector]
        public int leftHandGesture = -1;
        [HideInInspector]
        public int rightHandGesture = -1;

        private float[] movementDistance;
        private bool isPlayerMoving = false;

        private Vector3 leftMovement = Vector3.zero;
        private Vector3 rightMovement = Vector3.zero;

        //-----------Test interface (assign in Inspector to bypass VR tracking)-----------
        public Transform testHandObject;
        public Transform testHeadObject;
        private Vector3 testHandPos;
        private Vector3 testHeadPos;
        private Quaternion testHeadRot;

        private bool IsTestMode()
        {
            return testHandObject != null && testHeadObject != null;
        }

        public const int GESTURE_HAND_CASH_UP = 0;
        public const int GESTURE_HAND_CASH_DOWN = 1;
        public const int GESTURE_HAND_CASH_LEFT = 2;
        public const int GESTURE_HAND_CASH_RIGHT = 3;
        public const int GESTURE_HAND_CASH_FORWARD = 4;
        public const int GESTURE_HAND_CASH_BACKWARD = 5;
        public const int GESTURE_HAND_CASH_UP_LEFT = 6;
        public const int GESTURE_HAND_CASH_UP_RIGHT = 7;
        public const int GESTURE_HAND_CASH_DOWN_LEFT = 8;
        public const int GESTURE_HAND_CASH_DOWN_RIGHT = 9;

        private void UpdateGestureControl()
        {
            if (VRC.SDKBase.Networking.LocalPlayer == null)
                return;
            if (VRC.SDKBase.Networking.LocalPlayer.IsUserInVR() || true)
            {
                PopulateFields();

                if (isPlayerMoving == false)
                {
                    if (DidSwipeWithGesture(HAND_RIGHT, Vector3.down, GESTURE_HAND_CASH_DOWN, REQUIRED_OPENING_DISTANCE))
                    {
                        PushGestureToHistory(GESTURE_HAND_CASH_DOWN);
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, Vector3.up, GESTURE_HAND_CASH_UP, REQUIRED_OPENING_DISTANCE))
                    {
                        PushGestureToHistory(GESTURE_HAND_CASH_UP);
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, Vector3.left, GESTURE_HAND_CASH_LEFT, REQUIRED_OPENING_DISTANCE))
                    {
                        PushGestureToHistory(GESTURE_HAND_CASH_LEFT);
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, Vector3.right, GESTURE_HAND_CASH_RIGHT, REQUIRED_OPENING_DISTANCE))
                    {
                        PushGestureToHistory(GESTURE_HAND_CASH_RIGHT);
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, DIR_UP_LEFT, GESTURE_HAND_CASH_UP_LEFT, REQUIRED_OPENING_DISTANCE))
                    {
                        PushGestureToHistory(GESTURE_HAND_CASH_UP_LEFT);
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, DIR_UP_RIGHT, GESTURE_HAND_CASH_UP_RIGHT, REQUIRED_OPENING_DISTANCE))
                    {
                        PushGestureToHistory(GESTURE_HAND_CASH_UP_RIGHT);
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, DIR_DOWN_LEFT, GESTURE_HAND_CASH_DOWN_LEFT, REQUIRED_OPENING_DISTANCE))
                    {
                        PushGestureToHistory(GESTURE_HAND_CASH_DOWN_LEFT);
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, DIR_DOWN_RIGHT, GESTURE_HAND_CASH_DOWN_RIGHT, REQUIRED_OPENING_DISTANCE))
                    {
                        PushGestureToHistory(GESTURE_HAND_CASH_DOWN_RIGHT);
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, Vector3.forward, GESTURE_HAND_CASH_FORWARD, REQUIRED_OPENING_DISTANCE))
                    {
                        PushGestureToHistory(GESTURE_HAND_CASH_FORWARD);
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, Vector3.back, GESTURE_HAND_CASH_BACKWARD, REQUIRED_OPENING_DISTANCE))
                    {
                        PushGestureToHistory(GESTURE_HAND_CASH_BACKWARD);
                    }
                }
            }
        }

        private void PopulateFields()
        {
            prev_head = head;
            prev_rightHand = rightHand;
            prev_leftHand = leftHand;
            prev_speed_cal_time = speed_cal_time;

            if (IsTestMode())
            {
                Vector3 prevTestHandPos = testHandPos;
                Vector3 prevTestHeadPos = testHeadPos;

                testHandPos = testHandObject.position;
                testHeadPos = testHeadObject.position;
                testHeadRot = testHeadObject.rotation;

                speed_cal_time = Time.time;
                time_delta = Time.deltaTime;
                isPlayerMoving = false;
                leftHandGesture = GESTURE_HAND_NULL;
                rightHandGesture = GESTURE_HAND_NULL;
                s_Gesture = "TEST MODE";

                leftMovement = testHandPos - prevTestHandPos + prevTestHeadPos - testHeadPos;
                rightMovement = testHandPos - prevTestHandPos + prevTestHeadPos - testHeadPos;
            }
            else
            {
                rightHand = VRC.SDKBase.Networking.LocalPlayer.GetTrackingData(VRC.SDKBase.VRCPlayerApi.TrackingDataType.RightHand);
                leftHand = VRC.SDKBase.Networking.LocalPlayer.GetTrackingData(VRC.SDKBase.VRCPlayerApi.TrackingDataType.LeftHand);
                head = VRC.SDKBase.Networking.LocalPlayer.GetTrackingData(VRC.SDKBase.VRCPlayerApi.TrackingDataType.Head);

                speed_cal_time = Time.time;
                time_delta = Time.deltaTime;
                isPlayerMoving = IsPlayerMoving();
                leftHandGesture = GetHandGesture(HAND_LEFT);
                rightHandGesture = GetHandGesture(HAND_RIGHT);
                s_Gesture = "L: " + leftHandGesture + " R: " + rightHandGesture;
                // ControllerInputUI.Trigger;
                leftMovement = leftHand.position - prev_leftHand.position + prev_head.position - head.position;
                rightMovement = rightHand.position - prev_rightHand.position + prev_head.position - head.position;
            }
        }

        private float[] movementCache = new float[20];
        private bool DidSwipeWithGesture(int hand, Vector3 direction, int cacheIndex, float requiredDistance)
        {
            if(cacheIndex >= movementCache.Length)
            {
                return false;
            }
            
            Vector3 worldMovement = hand == HAND_RIGHT ? rightMovement : leftMovement;
            Quaternion headRot = IsTestMode() ? testHeadRot : head.rotation;
            Vector3 movement = Quaternion.Inverse(headRot) * worldMovement;

            if (Vector3.Angle(direction, movement) >= MAX_ANGLE) 
            { 
                movementCache[cacheIndex] = 0; 
                return false; 
            }

            bool isRight = hand == HAND_RIGHT;
            if (!IsTestMode() && Networking.LocalPlayer.GetPickupInHand(isRight ? VRC_Pickup.PickupHand.Right : VRC_Pickup.PickupHand.Left) != null) 
            { 
                movementCache[cacheIndex] = 0; 
                return false; 
            }

            //if (!IsTestMode() && !IsLookingAtHand(isRight ? rightHand : leftHand)) 
            //{ 
            //    movementCache[cacheIndex] = 0; 
            //    return false; 
            //}

            if (movement.magnitude / time_delta <= REQUIRED_MOVE_SPEED * local_player_height) 
            { 
                movementCache[cacheIndex] = 0; 
                return false; 
            }

            movementCache[cacheIndex] += movement.magnitude;
            if (movementCache[cacheIndex] > requiredDistance * local_player_height)
            {
                movementCache[cacheIndex] = 0;
                return true;
            }

            return false;
        }

        const float GESTURE_NO_PRESS_MAX_BUTTON_FORCE = 0.6f;
        const float GESTURE_REQUIRED_BUTTON_FORCE = 0.5f;
        const float GESTURE_NO_PRESS_MIN_BUTTON_FORCE = 0.4f;
        
        const int GESTURE_HAND_NULL = 0;
        const int GESTURE_HAND_OPEN = 1;
        const int GESTURE_FINGER_POINT = 2;
        const int GESTURE_HAND_FIST = 3;
        const int GESTURE_HAND_CLICK = 4;

        private const string OCULUS_INDEX_TRIGGER_R = "Oculus_CrossPlatform_SecondaryIndexTrigger";
        private const string OCULUS_INDEX_TRIGGER_L = "Oculus_CrossPlatform_PrimaryIndexTrigger";

        private const string OCULUS_HAND_TRIGGER_R = "Oculus_CrossPlatform_SecondaryHandTrigger";
        private const string OCULUS_HAND_TRIGGER_L = "Oculus_CrossPlatform_PrimaryHandTrigger";

        private int GetHandGesture(int hand)
        {
            float handTrigger;
            float indexTrigger;
            if (hand == HAND_LEFT)
            {
                handTrigger = UnityEngine.Input.GetAxis(OCULUS_HAND_TRIGGER_L);
                indexTrigger = UnityEngine.Input.GetAxis(OCULUS_INDEX_TRIGGER_L);
            }
            else
            {
                handTrigger = UnityEngine.Input.GetAxis(OCULUS_HAND_TRIGGER_R);
                indexTrigger = UnityEngine.Input.GetAxis(OCULUS_INDEX_TRIGGER_R);
            }

            if (handTrigger > GESTURE_REQUIRED_BUTTON_FORCE && indexTrigger < GESTURE_NO_PRESS_MAX_BUTTON_FORCE)
                return GESTURE_FINGER_POINT;

            if (handTrigger < GESTURE_NO_PRESS_MIN_BUTTON_FORCE && indexTrigger < GESTURE_NO_PRESS_MAX_BUTTON_FORCE)
                return GESTURE_HAND_OPEN;

            if (handTrigger > GESTURE_REQUIRED_BUTTON_FORCE && indexTrigger > GESTURE_REQUIRED_BUTTON_FORCE)
                return GESTURE_HAND_FIST;

            if (handTrigger < GESTURE_NO_PRESS_MIN_BUTTON_FORCE && indexTrigger > GESTURE_REQUIRED_BUTTON_FORCE)
                return GESTURE_HAND_CLICK;
            return -1;
        }

        private bool IsLookingAtHand(VRCPlayerApi.TrackingData hand)
        {
            Quaternion rotation = Quaternion.LookRotation(hand.position - head.position, Vector3.up);
            Quaternion headrot = Quaternion.LookRotation(head.rotation * Vector3.forward, Vector3.up);
            Quaternion difference = headrot * Quaternion.Inverse(rotation);
            bool result = (difference.eulerAngles.x > 320 || difference.eulerAngles.x < 40) && (difference.eulerAngles.y > 325 || difference.eulerAngles.y < 35);
            return result;
        }

        private bool IsPlayerMoving()
        {
            return Networking.LocalPlayer.GetVelocity().magnitude > 0.1f;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private float GetAvatarHeight(VRCPlayerApi player)
        {
            float height = 0;
            Vector3 postition1 = player.GetBonePosition(HumanBodyBones.Head);
            Vector3 postition2 = player.GetBonePosition(HumanBodyBones.Neck);
            height += (postition1 - postition2).magnitude;
            postition1 = postition2;
            postition2 = player.GetBonePosition(HumanBodyBones.Hips);
            height += (postition1 - postition2).magnitude;
            postition1 = postition2;
            postition2 = player.GetBonePosition(HumanBodyBones.RightLowerLeg);
            height += (postition1 - postition2).magnitude;
            postition1 = postition2;
            postition2 = player.GetBonePosition(HumanBodyBones.RightFoot);
            height += (postition1 - postition2).magnitude;
            return height;
        }

        private float GetLocalAvatarHeight()
        {
            if (Networking.LocalPlayer == null)
                return 1;
            return Networking.LocalPlayer.GetAvatarEyeHeightAsMeters();

            return GetAvatarHeight(Networking.LocalPlayer);
        }

        public Text text;

        public void LogMsg(string msg)
        {
            if (text != null)
            {
                text.text = msg;
            }
            Debug.Log(msg);
        }
    }
}


