 
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class HopeUdonEvnApi : UdonSharpBehaviour
    {

        public HopeUdonFramework hugf;

        void Start()
        {
            if (hugf == null)
            {
                hugf = this.GetComponentInParent<HopeUdonFramework>();
            }
        }

        void Update()
        {
            ;
        }

        public void ToggleEvn_JoinGame(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("JoinGameCall", idx);
        }
        public void ToggleEvn_ExitGame(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("ExitGameCall", idx);
        }
        public void ToggleEvn_AddBlood(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("AddBloodCall", idx);
        }
        public void ToggleEvn_DecBlood(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("DecBloodCall", idx);
        }
        public void ToggleEvn_AddMaxBlood(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("AddMaxBloodCall", idx);
        }
        public void ToggleEvn_DecMaxBlood(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("DecMaxBloodCall", idx);
        }

        public void ToggleEvn_CoverId(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("ToggleEvn_CoverId", idx);
        }

        public void ToggleEvn_GameStart()
        {
            hugf.udonEvn.TriggerEvent("GameStartCall");
        }
        public void ToggleEvn_GameReset()
        {
            hugf.udonEvn.TriggerEvent("GameResetCall");
        }

        public void ToggleEvn_GuanXinEn(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("GuanXinEnCall", idx);
        }
        public void ToggleEvn_WuGuEn(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("WuGuENCall", idx);
        }

        public void ToggleEvn_CollentionCard()
        {
            hugf.udonEvn.TriggerEvent("CollentionCardCall");
        }
        public void ToggleEvn_StartGrabCard()
        {
            hugf.udonEvn.TriggerEvent("StartGrabCardCall");
        }
        public void ToggleEvn_TableGetCardJudge()
        {
            hugf.udonEvn.TriggerEvent("TableGetCardJudgeCall");
        }
        public void ToggleEvn_HandJudge(int x)
        {
            hugf.udonEvn.TriggerEvent("TableGetCardJudgeCall");
        }
        public void ToggleEvn_StartChooseGener()
        {
            hugf.udonEvn.TriggerEvent("StartChooseGenerCall");
        }

        public void ToggleEvn_OrganizeHandCard(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("OrganizeHandCardCall", idx);
        }
        public void ToggleEvn_ShullfHandCard(int idx)
        {
            hugf.udonEvn.TriggerEventWithData("ShullfHandCardCall", idx);
        }

        public void ToggleEvn_OnPeopleAdd()
        {
            hugf.udonEvn.TriggerEvent("OnPeopleAddCall");
        }

        public void ToggleEvn_OnPeopleDec()
        {
            hugf.udonEvn.TriggerEvent("OnPeopleDecCall");
        }

        // end no idx
        public void ToggleEvn_JoinGame_0() { ToggleEvn_JoinGame(0); }
		public void ToggleEvn_ExitGame_0() { ToggleEvn_ExitGame(0); }
		public void ToggleEvn_AddBlood_0() { ToggleEvn_AddBlood(0); }
		public void ToggleEvn_DecBlood_0() { ToggleEvn_DecBlood(0); }
		public void ToggleEvn_HandJudge_0() { ToggleEvn_HandJudge(0); }
		public void ToggleEvn_AddMaxBlood_0() { ToggleEvn_AddMaxBlood(0); }
		public void ToggleEvn_DecMaxBlood_0() { ToggleEvn_DecMaxBlood(0); }
		public void ToggleEvn_CoverId_0() { ToggleEvn_CoverId(0); }
		public void ToggleEvn_OrganizeHandCard_0() { ToggleEvn_OrganizeHandCard(0); }
		public void ToggleEvn_ShullfHandCard_0() { ToggleEvn_ShullfHandCard(0); }
		public void ToggleEvn_JoinGame_1() { ToggleEvn_JoinGame(1); }
		public void ToggleEvn_ExitGame_1() { ToggleEvn_ExitGame(1); }
		public void ToggleEvn_AddBlood_1() { ToggleEvn_AddBlood(1); }
		public void ToggleEvn_DecBlood_1() { ToggleEvn_DecBlood(1); }
		public void ToggleEvn_HandJudge_1() { ToggleEvn_HandJudge(1); }
		public void ToggleEvn_AddMaxBlood_1() { ToggleEvn_AddMaxBlood(1); }
		public void ToggleEvn_DecMaxBlood_1() { ToggleEvn_DecMaxBlood(1); }
		public void ToggleEvn_CoverId_1() { ToggleEvn_CoverId(1); }
		public void ToggleEvn_OrganizeHandCard_1() { ToggleEvn_OrganizeHandCard(1); }
		public void ToggleEvn_ShullfHandCard_1() { ToggleEvn_ShullfHandCard(1); }
		public void ToggleEvn_JoinGame_2() { ToggleEvn_JoinGame(2); }
		public void ToggleEvn_ExitGame_2() { ToggleEvn_ExitGame(2); }
		public void ToggleEvn_AddBlood_2() { ToggleEvn_AddBlood(2); }
		public void ToggleEvn_DecBlood_2() { ToggleEvn_DecBlood(2); }
		public void ToggleEvn_HandJudge_2() { ToggleEvn_HandJudge(2); }
		public void ToggleEvn_AddMaxBlood_2() { ToggleEvn_AddMaxBlood(2); }
		public void ToggleEvn_DecMaxBlood_2() { ToggleEvn_DecMaxBlood(2); }
		public void ToggleEvn_CoverId_2() { ToggleEvn_CoverId(2); }
		public void ToggleEvn_OrganizeHandCard_2() { ToggleEvn_OrganizeHandCard(2); }
		public void ToggleEvn_ShullfHandCard_2() { ToggleEvn_ShullfHandCard(2); }

		public void ToggleEvn_JoinGame_4() { ToggleEvn_JoinGame(4); }
		public void ToggleEvn_ExitGame_4() { ToggleEvn_ExitGame(4); }
		public void ToggleEvn_AddBlood_4() { ToggleEvn_AddBlood(4); }
		public void ToggleEvn_DecBlood_4() { ToggleEvn_DecBlood(4); }
		public void ToggleEvn_HandJudge_4() { ToggleEvn_HandJudge(4); }
		public void ToggleEvn_AddMaxBlood_4() { ToggleEvn_AddMaxBlood(4); }
		public void ToggleEvn_DecMaxBlood_4() { ToggleEvn_DecMaxBlood(4); }
		public void ToggleEvn_CoverId_4() { ToggleEvn_CoverId(4); }
		public void ToggleEvn_OrganizeHandCard_4() { ToggleEvn_OrganizeHandCard(4); }
		public void ToggleEvn_ShullfHandCard_4() { ToggleEvn_ShullfHandCard(4); }

		public void ToggleEvn_WuGuEn_0() { ToggleEvn_WuGuEn(0); }
		public void ToggleEvn_WuGuEn_1() { ToggleEvn_WuGuEn(1); }
		public void ToggleEvn_GuanXinEn_1() { ToggleEvn_GuanXinEn(1); }
		public void ToggleEvn_WuGuEn_2() { ToggleEvn_WuGuEn(2); }
		public void ToggleEvn_GuanXinEn_2() { ToggleEvn_GuanXinEn(2); }
		public void ToggleEvn_GuanXinEn_0() { ToggleEvn_GuanXinEn(0); }
		public void ToggleEvn_JoinGame_3() { ToggleEvn_JoinGame(3); }
		public void ToggleEvn_ExitGame_3() { ToggleEvn_ExitGame(3); }
		public void ToggleEvn_AddBlood_3() { ToggleEvn_AddBlood(3); }
		public void ToggleEvn_DecBlood_3() { ToggleEvn_DecBlood(3); }
		public void ToggleEvn_HandJudge_3() { ToggleEvn_HandJudge(3); }
		public void ToggleEvn_AddMaxBlood_3() { ToggleEvn_AddMaxBlood(3); }
		public void ToggleEvn_DecMaxBlood_3() { ToggleEvn_DecMaxBlood(3); }
		public void ToggleEvn_CoverId_3() { ToggleEvn_CoverId(3); }
		public void ToggleEvn_OrganizeHandCard_3() { ToggleEvn_OrganizeHandCard(3); }
		public void ToggleEvn_ShullfHandCard_3() { ToggleEvn_ShullfHandCard(3); }
		public void ToggleEvn_WuGuEn_3() { ToggleEvn_WuGuEn(3); }
		public void ToggleEvn_GuanXinEn_3() { ToggleEvn_GuanXinEn(3); }
		public void ToggleEvn_WuGuEn_4() { ToggleEvn_WuGuEn(4); }
		public void ToggleEvn_GuanXinEn_4() { ToggleEvn_GuanXinEn(4); }
		// end method
    }
}