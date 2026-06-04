Audio para SphereXRR
====================

Como asignar los audios
-----------------------
1. Los WAV deben estar dentro de Assets/Audio.
2. En el GameObject con MovementSequenceManager asigna:
   - moveSound
   - successSound
   - failSound
   - finalSound
   - soundtrackClip
3. Para la estrella fugaz selecciona VX_Shooting_Star.
4. En el componente ShootingStarAnimator asigna:
   - whooshClip: shootingStarWhoosh_optional.wav
   - whooshVolume: 0.25 a 0.40 recomendado.
5. El AudioSource de VX_Shooting_Star debe quedar:
   - Play On Awake: Off
   - Loop: Off
   - Volume: 0.38
   - Spatial Blend: 0.85

Prompt shootingStarWhoosh_optional.wav
--------------------------------------
"Create an airy shooting star whoosh for a deep space background, 900 ms to 1.2 seconds long, fast stereo pass-by, tiny glitter particles, very subtle, no voice, no music, game-ready WAV, 48 kHz."
