using UnityEngine;
using System;
using System.Collections.Generic;
using LuaInterface;

using BindType = ToLuaMenu.BindType;
using Logic.TalkingData.Controller;
using UnityEngine.UI;

public static class CustomSettings
{
    public static string saveDir = Application.dataPath + "/ToLua/Generate/";
    public static string luaDir = Application.dataPath + "/ToLua/Lua/";
    public static string toluaBaseType = Application.dataPath + "/ToLua/BaseType/";
    public static string toluaLuaDir = Application.dataPath + "/ToLua/Lua";

    //导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
    //unity 有些类作为sealed class, 其实完全等价于静态类
    public static List<Type> staticClassTypes = new List<Type>
    {        
        typeof(UnityEngine.Application),
        typeof(UnityEngine.Time),
        typeof(UnityEngine.Screen),
        typeof(UnityEngine.SleepTimeout),
        typeof(UnityEngine.Input),
        typeof(UnityEngine.Resources),
        typeof(UnityEngine.Physics),
        //typeof(UnityEngine.RectTransform),
    };

    //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
    public static DelegateType[] customDelegateList = 
    {        
        _DT(typeof(Action)),
        _DT(typeof(Action<GameObject>)),
        _DT(typeof(Action<int>)),
        _DT(typeof(Action<float>)),
		_DT(typeof(Action<Vector3>)),
		_DT(typeof(System.Action<Logic.Character.PlayerEntity>)),
		_DT(typeof(System.Action<Logic.Character.PetEntity>)),
		_DT(typeof(System.Action<Logic.Character.HeroEntity>)),
		
        _DT(typeof(UnityEngine.Events.UnityAction)),
		_DT(typeof(UnityEngine.Events.UnityAction<int>)),
		_DT(typeof(UnityEngine.Events.UnityAction<string>)),
		_DT(typeof(UnityEngine.Events.UnityAction<Vector2>)),
        _DT(typeof(UnityEngine.Events.UnityAction<GameObject,int>)),       
        _DT(typeof(UnityEngine.Events.UnityAction<GameObject>)),       
    };

    //在这里添加你要导出注册到lua的类型列表
    public static BindType[] customTypeList = 
    {                
        //------------------------为例子导出--------------------------------
        //_GT(typeof(TestEventListener)),                
        //_GT(typeof(TestAccount)),
        //_GT(typeof(Dictionary<int, TestAccount>)).SetLibName("AccountMap"),                
        //_GT(typeof(KeyValuePair<int, TestAccount>)),    
        //-------------------------------------------------------------------
        _GT(typeof(Debugger)), 
                      
        _GT(typeof(Canvas)),
        _GT(typeof(Resources)),                               
        _GT(typeof(ToLuaPb)),                               
        _GT(typeof(ToLuaProtol)),
        _GT(typeof(Component)),
        _GT(typeof(Behaviour)),
        _GT(typeof(MonoBehaviour)),        
        _GT(typeof(GameObject)),
        _GT(typeof(RectTransform)),
		_GT(typeof(Rect)),
        _GT(typeof(Sprite)),
        _GT(typeof(Camera)),   
		_GT(typeof(UnityEngine.Random)),
        //_GT(typeof(CameraClearFlags)),           
        _GT(typeof(Material)),
        _GT(typeof(Renderer)), 
		_GT(typeof(ParticleSystemRenderer)),
        //_GT(typeof(MeshRenderer)),
        //_GT(typeof(SkinnedMeshRenderer)),
        //_GT(typeof(Light)),
        //_GT(typeof(LightType)),                             
        //_GT(typeof(ParticleSystem)),                
        //_GT(typeof(Physics)),
        //_GT(typeof(Collider)),
        //_GT(typeof(BoxCollider)),
        //_GT(typeof(MeshCollider)),
        //_GT(typeof(SphereCollider)),        
        //_GT(typeof(CharacterController)),
        //_GT(typeof(Animation)),             
        //_GT(typeof(AnimationClip)),
        //_GT(typeof(TrackedReference)),
        //_GT(typeof(AnimationState)),  
        //_GT(typeof(QueueMode)),  
        //_GT(typeof(PlayMode)),                          
        _GT(typeof(AudioClip)),
        _GT(typeof(AudioSource)),    
		_GT(typeof(AudioRolloffMode)),
        _GT(typeof(Application)),
        //_GT(typeof(Input)),              
        //_GT(typeof(KeyCode)),             
        //_GT(typeof(Screen)),
        _GT(typeof(Time)),
        //_GT(typeof(RenderSettings)),
        //_GT(typeof(SleepTimeout)),                        
        //_GT(typeof(AsyncOperation)),
        //_GT(typeof(AssetBundle)),   
        //_GT(typeof(BlendWeights)),   
        //_GT(typeof(QualitySettings)),          
        //_GT(typeof(AnimationBlendMode)),  
        //_GT(typeof(RenderTexture)),
        //_GT(typeof(Rigidbody)), 
        //_GT(typeof(CapsuleCollider)),
        //_GT(typeof(WrapMode)),
        //_GT(typeof(Texture)),
        //_GT(typeof(Shader)),
        //_GT(typeof(Texture2D)),
        //_GT(typeof(WWW)),
		_GT(typeof(UITextOutline)),
		_GT(typeof(Quaternion)),
		_GT(typeof(System.DateTime)),
        _GT(typeof(System.DayOfWeek)),
		_GT(typeof(System.TimeSpan)),
		_GT(typeof(PlayerPrefs)),
		_GT(typeof(Animator)),
        _GT(typeof(UnityEngine.UI.Image)),
        _GT(typeof(UnityEngine.UI.Text)),
        _GT(typeof(UnityEngine.UI.Button)),
        _GT(typeof(UnityEngine.UI.Toggle)),
        _GT(typeof(UnityEngine.UI.Slider)),
		_GT(typeof(UnityEngine.UI.InputField)),
        _GT(typeof(UnityEngine.UI.Outline)),
        _GT(typeof(UnityEngine.UI.Dropdown)),
		_GT(typeof(UnityEngine.UI.Dropdown.DropdownEvent)),
		_GT(typeof(UnityEngine.UI.Dropdown.OptionData)),
        _GT(typeof(UnityEngine.UI.Button.ButtonClickedEvent)),
		_GT(typeof(UnityEngine.UI.Toggle.ToggleEvent)),
		_GT(typeof(UnityEngine.UI.GridLayoutGroup)),
		_GT(typeof(UnityEngine.UI.LayoutElement)),
		_GT(typeof(UnityEngine.UI.InputField.OnChangeEvent)),
        _GT(typeof(UnityEngine.UI.Scrollbar)),
        _GT(typeof(UnityEngine.UI.ScrollRect)),
        _GT(typeof(UnityEngine.UI.ScrollRect.ScrollRectEvent)),
      	_GT(typeof(LuaCsTransfer)),
		_GT(typeof(UnityEngine.UI.InputField.SubmitEvent)),
      	_GT(typeof(LeanTween)),
		_GT(typeof(LTDescr)),
		_GT(typeof(LeanTweenType)),
		_GT(typeof(LitJson.JsonData)),
        _GT(typeof(Observers.Facade)),
		_GT(typeof(Observers.Notification)),
        _GT(typeof(Common.Localization.LocalizationController)),
        _GT(typeof(Common.ResMgr.ResMgr)),
		_GT(typeof(Logic.Shaders.ShadersUtil)),
		_GT(typeof(Logic.UI.UIMgr)),
		_GT(typeof(Logic.UI.CommonAnimations.CommonMoveByAnimation)),
		_GT(typeof(Logic.UI.CommonAnimations.CommonFadeInAnimation)),
		_GT(typeof(Logic.UI.CommonAnimations.CommonFadeToAnimation)),
		_GT(typeof(Logic.Audio.Controller.AudioController)),
        _GT(typeof(Logic.Pool.Controller.PoolController)),
		_GT(typeof(Common.Animators.AnimatorUtil)),
		_GT(typeof(Logic.Skill.SkillUtil)),
        _GT(typeof(Logic.Character.CharacterEntity)),
        _GT(typeof(Logic.Character.HeroEntity)),
        _GT(typeof(Logic.Character.PlayerEntity)),
        _GT(typeof(Logic.Character.EnemyEntity)),
        _GT(typeof(Logic.Character.EnemyPlayerEntity)),
        _GT(typeof(Logic.Character.PetEntity)),
        _GT(typeof(Logic.Character.Model.CharacterBaseInfo)),
        _GT(typeof(Logic.Character.Model.BuffInfo)),
        _GT(typeof(Logic.Character.Controller.PlayerController)),
        _GT(typeof(Logic.Character.Controller.EnemyController)),
        _GT(typeof(Logic.Fight.Controller.MechanicsController)),
        _GT(typeof(Logic.Skill.Model.SkillInfo)),
        _GT(typeof(Logic.Skill.Model.SkillData)),
        _GT(typeof(Logic.Skill.Model.SkillDesInfo)),
        _GT(typeof(Logic.Skill.Model.MechanicsData)),
        _GT(typeof(Logic.Position.Model.PositionData)),
        _GT(typeof(List<Logic.Skill.Model.SkillDesInfo>)),
        _GT(typeof(Logic.Enums.SkillType)),
        _GT(typeof(Logic.Enums.BuffType)),
        _GT(typeof(Logic.Enums.SkillLevelBuffAddType)),
        _GT(typeof(Logic.Enums.BuffAddType)),
        _GT(typeof(Logic.Enums.MechanicsType)),
        _GT(typeof(Logic.Enums.RoleAttackAttributeType)),
        _GT(typeof(Logic.Enums.TargetType)),
        _GT(typeof(Logic.Enums.MechanicsValueType)),
		_GT(typeof(Logic.Enums.BaseResType)),
		_GT(typeof(Logic.UI.SoftGuide.Model.SoftGuideProxy)),
		_GT(typeof(Logic.Model.View.ModelRotateAndAnim)),
		_GT(typeof(Logic.Action.Controller.ActionController)),
		_GT(typeof(Common.Animators.AnimationUtil)),
		_GT(typeof(Logic.Game.Model.GameResData)),
		_GT(typeof(TalkingDataController)),
		_GT(typeof(Logic.AdTracking.Controller.AdTrackingController)),
		
		_GT(typeof(Common.Util.TimeUtil)),
		_GT(typeof(Common.Util.TransformUtil)),
		_GT(typeof(Common.UI.Components.SliderValueChangeAction)),
		_GT(typeof(Common.UI.Components.ScrollContentExpand)),
		_GT(typeof(Common.UI.Components.ScrollContent)),
		_GT(typeof(Common.UI.Components.OnResetItem)),
		_GT(typeof(Common.UI.Components.ToggleContent)),
		_GT(typeof(Common.UI.Components.OnInitComplete)),
		_GT(typeof(Common.UI.Components.EventTriggerDelegate)),
		_GT(typeof(Common.UI.Components.UnityEventGameObject)),
		_GT(typeof(Common.Util.BlackListWordUtil)),
		_GT(typeof(Logic.UI.Friend.Model.FriendInfo)),
		_GT(typeof(Logic.UI.RedPoint.View.RedPointView)),
		_GT(typeof(Logic.UI.FightResult.View.NumberIncreaseAction)),
        _GT(typeof(Common.GameTime.Controller.TimeController)),
        _GT(typeof(Dictionary<uint, Logic.Character.HeroEntity>)),                
        _GT(typeof(KeyValuePair<uint,  Logic.Character.HeroEntity>)), 
        _GT(typeof(Dictionary<uint, Logic.Character.EnemyEntity>)),                
        _GT(typeof(KeyValuePair<uint,  Logic.Character.EnemyEntity>)),
		_GT(typeof(KeyValuePair<int,float>)),
		_GT(typeof(List<KeyValuePair<int,float>>)),
		_GT(typeof(List<UnityEngine.UI.Dropdown.OptionData>)),
		_GT(typeof(List<Logic.Game.Model.GameResData>)),
		_GT(typeof(Logic.UI.GoodsJump.View.GoodsJumpButton)),
    };

    public static List<Type> dynamicList = new List<Type>()
    {
        //typeof(MeshRenderer),
        //typeof(ParticleEmitter),
        //typeof(ParticleRenderer),
        //typeof(ParticleAnimator),

        //typeof(BoxCollider),
        //typeof(MeshCollider),
        //typeof(SphereCollider),
        //typeof(CharacterController),
        //typeof(CapsuleCollider),

        //typeof(Animation),
        //typeof(AnimationClip),
        //typeof(AnimationState),        

        //typeof(BlendWeights),
        //typeof(RenderTexture),
        //typeof(Rigidbody),       
    };

    //重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
    //使用方法参见例子14
    public static List<Type> outList = new List<Type>()
    {

    };

    static BindType _GT(Type t)
    {
        return new BindType(t);
    }

    static DelegateType _DT(Type t)
    {
        return new DelegateType(t);
    }
}
