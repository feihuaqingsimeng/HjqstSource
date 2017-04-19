local t = {}
local name = 'audio_model'

local daily_audio_data = gamemanager.GetData('daily_audio_data')

local function Start()
  gamemanager.RegisterModel(name,t)
end

function t.isOpenAudio()
  return AudioController.instance.isOpenAudio
end
function t.isOpenAudioBg()
  return AudioController.instance.isOpenAudioBg
end
function t.GetTransform()
  return AudioController.instance.transform
end
function t.Play(name)
  AudioController.instance:PlayAudio( name,false, 0)
end

--界面英雄声音
function t.PlayRandomAudioInView(audioViewType,modelId,delayPlay)
  if modelId == nil then
    modelId = 0
  end
  if delayPlay == nil then
    delayPlay = 0
  end
  local data = daily_audio_data.GetDataByTypeHeroId(audioViewType,modelId)
  if data == nil then
    return 
  end
  print('data.audioList',#data.audioList)
  local transform = t.GetTransform()
  for k,v in pairs(data.audioList) do
    if transform:Find(v) then
      return
    end
  end
  
  local length = #data.audioList
  local index = UnityEngine.Random.Range(1,length)
  local playAudioType = nil
  
  coroutine.start(t.PlayAudioCoroutine,data.audioList[index],delayPlay)
end


function t.PlayAudioCoroutine(name, delay)
  if not t.isOpenAudio() then
    return
  end
  coroutine.wait(delay)
  local clip = ResMgr.instance:Load("audio/" .. name)
  if clip == nil then
    return
  end
  print('PlayAudioCoroutine')
  local go = nil
  local isCreate = true
  local transform = t.GetTransform()
  
  if (isCreate) then
    go = GameObject.New();
    go.name = name;
    local audioSource = go:AddComponent(typeof(AudioSource))
    go.transform:SetParent(transform, false)
    audioSource.clip = clip;
    audioSource.rolloffMode = UnityEngine.AudioRolloffMode.Linear
    audioSource.pitch = 1
    audioSource.volume = 0.9
    audioSource:Play()
    local length = clip.length
    if (audioSource) then
      UnityEngine.Object.Destroy(go,length)
    end
  end
end

Start()
return t