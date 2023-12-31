﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace GameScene
{
    public class MoneyCounterController : MonoBehaviour
    {
        public static MoneyCounterController Instance;
        public TextMeshProUGUI Label;

        private readonly string AllGrades = " KMBTQq";
        private string MoneyStringConventer(long value)
        {
            int DigitCount = value.ToString().Length;
            int Grade = (int)((float)(DigitCount - 1f) / 3f);
            char GradeChar = AllGrades[Grade];

            decimal ShortedNumber = value;
            ShortedNumber /= (decimal)Mathf.Pow(1000, Grade);
            string OutputNumber = ShortedNumber.ToString();

            if (OutputNumber.Length > 6)
                return OutputNumber.Remove(6, OutputNumber.Length - 6) + GradeChar;
            else return OutputNumber + GradeChar;
        }
        public void AddMoney(int number = 1)
        {
            if(GameInfo.Instance!=null)
                GameInfo.Instance.PlayerMoney += number;
            SetState();
            LeanTween.moveLocalY(gameObject, transform.localPosition.y + 10, 0.05f).setEaseInOutCubic().setOnComplete(()=> {
                LeanTween.moveLocalY(gameObject, transform.localPosition.y - 20, 0.05f).setEaseInOutCubic().setOnComplete(() => {
                    LeanTween.moveLocalY(gameObject, transform.localPosition.y + 10, 0.05f).setEaseInOutCubic();
                });
            });
        }
        /// <summary>
        /// With animation
        /// </summary>
        /// <param name="number"></param>
        public bool SubMoney(int number = 1) 
        {
            long A = GameInfo.Instance.PlayerMoney;
            if (GameInfo.Instance != null&&A>=number)
            {
                GameInfo.Instance.PlayerMoney -= number;
                LeanTween.value(gameObject, (float)A, GameInfo.Instance.PlayerMoney, 1f).setOnUpdate((float val) =>
                {
                    Label.text = MoneyStringConventer((int)val);
                }).setEaseOutCubic();
                return true;
            }
            return false;
        }
        private void SetState()
        {
            if (GameInfo.Instance != null)
            {
                Label.text = MoneyStringConventer(GameInfo.Instance.PlayerMoney);
            }
        }
        public Vector2 GetPosition()
        {
            return gameObject.transform.position;
        }
        void Start()
        {
            //Stetting up
            SetState();
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
    }
}

