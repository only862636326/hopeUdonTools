
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class HandGesture : UdonSharpBehaviour
    {
        const float SLOW_UPDATE_RATE = 1.0f;
        private float last_slow_update = 0;
        public TextMeshProUGUI meshPro;
        public string s_Gesture;
        public string move_Gest;

        private void Start()
        {
            local_player_height = GetLocalAvatarHeight();
            meshPro.text = "test mesh pro";
        }

        private void Update()
        {
            UpdateGestureControl();

            if (Time.time - last_slow_update > SLOW_UPDATE_RATE)
            {
                last_slow_update = Time.time;
                SlowUpdate();
            }
        }

        private void SlowUpdate()
        {
            local_player_height = GetLocalAvatarHeight();
            this.meshPro.text = s_Gesture + "\n" + move_Gest;
            move_Gest = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public float GetAvatarHeight(VRCPlayerApi player)
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
            return GetAvatarHeight(Networking.LocalPlayer);
        }

        //-----------Gesture tracking---------------

        private const string OCULUS_INDEX_TRIGGER_R = "Oculus_CrossPlatform_SecondaryIndexTrigger";
        private const string OCULUS_INDEX_TRIGGER_L = "Oculus_CrossPlatform_PrimaryIndexTrigger";

        private const string OCULUS_HAND_TRIGGER_R = "Oculus_CrossPlatform_SecondaryHandTrigger";
        private const string OCULUS_HAND_TRIGGER_L = "Oculus_CrossPlatform_PrimaryHandTrigger";

        const float GESTURE_REQUIRED_BUTTON_FORCE = 0.5f;
        const float GESTURE_NO_PRESS_MAX_BUTTON_FORCE = 0.5f;
        const float GESTURE_NO_PRESS_MIN_BUTTON_FORCE = 0.5f;

        const int GESTURE_HAND_NULL = 0;
        const int GESTURE_HAND_OPEN = 1;
        const int GESTURE_FINGER_POINT = 2;
        const int GESTURE_HAND_FIST = 3;
        const int GESTURE_HAND_CLICK = 4;

        const int HAND_LEFT = 0;
        const int HAND_RIGHT = 1;

        public float MAX_ANGLE = 30.0f;

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


        private void UpdateGestureControl()
        {
            if (VRC.SDKBase.Networking.LocalPlayer == null)
                return;
            if (VRC.SDKBase.Networking.LocalPlayer.IsUserInVR() || true)
            {
                PopulateFields();

                if (isPlayerMoving == false)
                {
                    if (DidSwipeWithGesture(HAND_RIGHT, Vector3.down, GESTURE_HAND_NULL, true, 0, REQUIRED_OPENING_DISTANCE))
                    {
                        move_Gest += " V";
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, Vector3.up, GESTURE_HAND_NULL, true, 1, REQUIRED_OPENING_DISTANCE))
                    {
                        move_Gest += " A";
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, Vector3.left, GESTURE_HAND_NULL, true, 2, REQUIRED_OPENING_DISTANCE * 0.6f))
                    {
                        move_Gest += " <-";
                    }
                    if (DidSwipeWithGesture(HAND_RIGHT, Vector3.right, GESTURE_HAND_NULL, true, 3, REQUIRED_OPENING_DISTANCE * 0.6f))
                    {
                        move_Gest += " ->";
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

        private bool IsPlayerMoving()
        {
            return Networking.LocalPlayer.GetVelocity().magnitude > 0.1f;
        }

        /**
         * <param name="direction">Direction of the swipe to check</param>
         * <param name="gesture">Int related to gesture to check for</param>
         * <param name="gestureDefault">Gesture default value in case gesture could not be determined. If true swipe will be executed if gesture cannot be determined.</param>
         * */
        private bool DidSwipeWithGestureAnyHand(Vector3 direction, int gesture, bool gestureDefault, int cacheIndex, float requiredDistance)
        {
            return (DidSwipeWithGesture(HAND_LEFT, direction, gesture, gestureDefault, cacheIndex, requiredDistance) ||
                DidSwipeWithGesture(HAND_RIGHT, direction, gesture, gestureDefault, cacheIndex + 1, requiredDistance));
        }

        private float[] movementCache = new float[20];
        /**
         * <param name="hand">Hand to check for. possible values are HAND_LEFT, HAND_RIGHT</param>
         * <param name="direction">Direction of the swipe to check</param>
         * <param name="gesture">Int related to gesture to check for</param>
         * <param name="gestureDefault">Gesture default value in case gesture could not be determined. If true swipe will be executed if gesture cannot be determined.</param>
         * */
        private bool DidSwipeWithGesture(int hand, Vector3 direction, int gesture, bool gestureDefault, int cacheIndex, float requiredDistance)
        {

            Vector3 movement = hand == HAND_RIGHT ? rightMovement : leftMovement;
            float angle = Vector3.Angle(direction, movement);
            if (angle < MAX_ANGLE)
            {
                if (gesture == GESTURE_HAND_NULL || ((hand == HAND_RIGHT ? rightHandGesture : leftHandGesture) == gesture))
                {
                    if (Networking.LocalPlayer.GetPickupInHand(hand == HAND_RIGHT ? VRC_Pickup.PickupHand.Right : VRC_Pickup.PickupHand.Left) == null)
                    {
                        if (movement.magnitude / time_delta > REQUIRED_MOVE_SPEED * this.local_player_height)
                        {
                            movementCache[cacheIndex] += movement.magnitude;
                            if (movementCache[cacheIndex] > requiredDistance * local_player_height)
                            {
                                movementCache[cacheIndex] = 0;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            movementCache[cacheIndex] = 0;
                            return false;
                        }
                    }
                }
            }
            movementCache[cacheIndex] = 0;
            return false;
        }

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
    }

}