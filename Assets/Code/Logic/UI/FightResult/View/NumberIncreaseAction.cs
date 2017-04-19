using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Logic.UI.FightResult.View
{
    public class NumberIncreaseAction : MonoBehaviour
    {



        public Text text_num;
        public int startNumber = 0;
        public int endNumber = 100;
        public float duringTime = 2;
        public float delayTime = 0.2f;
        public bool reset;


        private bool _isOver = true;
        private int _curNumber;



        private int _addNumber;
        void Update()
        {

            if (reset)
            {
                reset = false;
                Init();
            }

            if (!_isOver)
            {

                _curNumber += _addNumber;
                if (_addNumber > 0 && _curNumber >= endNumber)
                {
                    _curNumber = endNumber;
                    _isOver = true;
                }
                else if (_addNumber < 0 && _curNumber <= endNumber)
                {
                    _curNumber = endNumber;
                    _isOver = true;
                }

                updateText();
            }
        }
        public void Init()
        {
            Init(startNumber, endNumber, delayTime);
        }
        public void Init(int starNumber, int endNumber, float delay = 0)
        {

            _addNumber = (int)((endNumber - starNumber) / (duringTime * 30));
            if (_addNumber == 0 && endNumber != starNumber)
                _addNumber = endNumber > starNumber ? 1 : -1;
            if (_addNumber == 0)
            {
                updateText();
                return;
            }
            this.startNumber = starNumber;
            this.endNumber = endNumber;
            _curNumber = starNumber;

            updateText();
            delayTime = delay;
            if (gameObject.activeInHierarchy)
                StartCoroutine(StartActionDelay());

        }
        private IEnumerator StartActionDelay()
        {
            yield return new WaitForSeconds(delayTime);
            _isOver = false;
        }
        private void updateText()
        {
            text_num.text = _curNumber.ToString();
        }
    }
}


