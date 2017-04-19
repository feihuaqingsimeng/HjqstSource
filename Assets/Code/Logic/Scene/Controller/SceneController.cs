using UnityEngine;
using System.Collections;
using Logic.Skill.Model;
using Logic.Net.Controller;
using Logic.Fight.Controller;
namespace Logic.Scene.Controller
{
    public class SceneController : SingletonMono<SceneController>
    {        
        void Awake()
        {
            instance = this;
        }
    }
}
