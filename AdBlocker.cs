using Modding;
using System.Collections.Generic;
using UnityEngine;
using Satchel;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;

namespace AdBlocker {
    public class AdBlocker: Mod, ITogglableMod {
        new public string GetName() => "AdBlocker";
        public override string GetVersion() => "1.0.0.0";

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            On.PlayMakerFSM.OnEnable += editFSM;
        }

        public void Unload() {
            On.PlayMakerFSM.OnEnable -= editFSM;
        }

        private void editFSM(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            //gruz mother
            if(self.gameObject.name == "Battle Scene" && self.FsmName == "Battle Control" && self.gameObject.scene.name == "Crossroads_04") {
                ((SetIntValue)self.GetState("Start").Actions[0]).intValue = 1;
            }
            else if(self.gameObject.name == "Corpse Big Fly Burster(Clone)" && self.FsmName == "burster") {
                FsmState spawnFlies2 = self.GetState("Spawn Flies 2");
                spawnFlies2.RemoveAction(1);
                SetIntValue endGruzFight = new();
                endGruzFight.intVariable = self.FsmVariables.GetFsmInt("Battle Enemies");
                endGruzFight.intValue = 0;
                spawnFlies2.AddAction(endGruzFight);
                spawnFlies2.AddAction(new sendGruzDeathEvent());
            }
            //vengefly king
            else if(self.gameObject.name.Contains("Giant Buzzer") && self.FsmName == "Big Buzzer") {
                self.ChangeTransition("Roar Right", "FINISHED", "Roar End");
                self.ChangeTransition("Roar Left", "FINISHED", "Roar End");
            }
            //false knight, failed champ
            else if(self.gameObject.name.Contains("False Knight") && self.FsmName == "FalseyControl") {
                self.GetState("S Attack Recover").RemoveAction(2);
                self.GetState("Turn").RemoveAction(0);
                for(int i = self.gameObject.name.Contains("Dream") ? 5 : 3; i >= 3; i--) {
                    self.GetState("JA Recoil").RemoveAction(i);
                }
            }
            //hornet 2
            else if(self.gameObject.name == "Hornet Boss 2" & self.FsmName == "Control") {
                self.GetState("Barb Throw").RemoveAction(1);
            }
            //flukemarm
            else if(self.gameObject.name == "Fluke Mother" && self.FsmName == "Fluke Mother") {
                for(int i = 7; i >= 4; i--) {
                    self.GetState("Spawn 2").RemoveAction(i);
                    self.GetState("Spawn").RemoveAction(i);
                }
            }
            //hive knight
            else if(self.gameObject.name == "Battle Scene" && self.FsmName == "Control" && self.gameObject.scene.name == "Hive_05") {
                self.GetState("Droppers").RemoveAction(2);
            }
            else if(self.gameObject.name == "Hive Knight" && self.FsmName == "Control") {
                self.GetState("Glob Strike").RemoveAction(3);
                self.GetState("Roar Recover").RemoveAction(0);
            }
            //broken vessel, lost kin
            else if((self.gameObject.name == "Infected Knight" || self.gameObject.name == "Lost Kin") && self.FsmName == "Spawn Balloon") {
                self.GetState("Spawn").RemoveAction(9);
                self.GetState("Spawn").RemoveAction(8);
            }
            //winged nosk
            else if(self.gameObject.name == "Hornet Nosk" && self.FsmName == "Hornet Nosk") {
                self.GetState("Summon").RemoveAction(7);
                self.GetState("Summon").RemoveAction(6);
            }
            //collector
            else if(self.gameObject.name == "Jar Collector" && self.FsmName == "Control") {
                FsmEvent skipEvent = new FsmEvent("SKIP SUMMON");
                self.AddTransition("Spawn", "SKIP SUMMON", "Spawn Recover");
                self.GetState("Spawn").InsertAction(new justSendEvent(skipEvent), 1);
            }
            //uumuu
            else if(self.gameObject.name == "Mega Jellyfish GG" && self.FsmName == "Mega Jellyfish") {
                self.GetState("Roar").RemoveAction(3);
            }
            //zote
            else if(self.gameObject.name == "Grey Prince" && self.FsmName == "Control") {
                self.ChangeTransition("Spit Dir", "L", "Spit Recover");
                self.ChangeTransition("Spit Dir", "R", "Spit Recover");
                self.GetState("B Roar").RemoveAction(4);
            }
            //soul warrior
            else if(self.FsmName == "Battle Control" && (self.gameObject.scene.name == "Ruins1_31b" || self.gameObject.scene.name.Contains("Mage_Knight"))) {
                self.GetState("Spawn").RemoveAction(1);
            }
            //no eyes
            else if(self.gameObject.name == "Ghost Warrior No Eyes" && self.FsmName == "Shot Spawn") {
                foreach(int i in new int[] { 6, 4, 2, 0 }){
                    self.GetState("Spawn L").RemoveAction(i);
                    self.GetState("Spawn R").RemoveAction(i);
                }
            }
            //galien
            else if(self.gameObject.name == "Ghost Warrior Galien" && self.FsmName == "Summon Minis") {
                self.GetState("Summon").RemoveAction(1);
                self.GetState("Summon 2").RemoveAction(1);
            }
            //markoth
            else if(self.gameObject.name == "Ghost Warrior Markoth" && self.FsmName == "Attacking") {
                self.GetState("Nail").RemoveAction(0);
            }
            //elder hu
            else if(self.gameObject.name == "Ghost Warrior Hu" && self.FsmName == "Attacking") {
                for(int i = 1; i <= 6; i++) {
                    self.ChangeTransition("Choice", i.ToString(), "Ring Antic");
                    self.ChangeTransition("Choice 2", i.ToString(), "Ring Antic");
                }
                self.GetState("M 1").RemoveAction(0);
                for(int i = 2; i <= 8; i++) {
                    self.GetState("M " + i.ToString()).RemoveAction(1);
                    self.GetState("M " + i.ToString()).RemoveAction(0);
                }
                self.GetState("M 9").RemoveAction(0);
            }
        }
    }

    public class sendGruzDeathEvent: FsmStateAction {
        public override void OnEnter() {
            GameObject.Find("Fly 1").GetComponent<HealthManager>().ApplyExtraDamage(99);
            Finish();
        }
    }

    public class justSendEvent: FsmStateAction {
        FsmEvent toEvent;
        public justSendEvent(FsmEvent targetEvent) {
            toEvent = targetEvent;
        }
        public override void OnEnter() {
            base.Fsm.Event(toEvent);
            Finish();
        }
    }
}