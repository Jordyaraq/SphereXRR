Sincronizacion de la cinematica Intro
=====================================

El MP4 puede reproducirse con el audio embebido, pero si Unity muestra warnings de timestamps o el audio se adelanta, usa audio externo:

1. Exporta o extrae el audio de Intro.mp4 como WAV:
   - Nombre recomendado: Intro_Audio.wav
   - Formato recomendado: WAV, 48 kHz, 16 o 24 bits, stereo.
2. Copia Intro_Audio.wav dentro de Assets/Cinematicas.
3. En Unity selecciona Intro_Cinematic_System.
4. En Intro Cinematic Controller asigna:
   - External Intro Audio: Intro_Audio.wav
   - External Audio Delay: empieza con 0.12
5. Ajusta External Audio Delay:
   - Si el audio se adelanta, sube el valor: 0.15, 0.18, 0.22.
   - Si el audio se retrasa, baja el valor: 0.08, 0.04, 0.

Cuando External Intro Audio esta asignado, el audio embebido del VideoPlayer queda silenciado automaticamente y se usa el AudioSource externo.

Como quitar los warnings AudioSampleProvider buffer overflow
-----------------------------------------------------------
Esos warnings salen cuando Unity intenta leer la pista de audio embebida del MP4. Para quitarlos:

1. Exporta el video sin audio o ignora su audio embebido.
2. Exporta el audio por separado como Intro_Audio.wav.
3. Asigna Intro_Audio.wav en External Intro Audio.

Cuando External Intro Audio tiene un clip asignado, el script cambia el VideoPlayer a VideoAudioOutputMode.None. Eso desactiva la pista de audio interna del MP4 y evita los warnings de AudioSampleProvider.

Recomendacion de exportacion del video
--------------------------------------
Para Unity y Meta Quest, lo mas estable suele ser:
- MP4
- H.264 baseline o main
- Constant frame rate
- 1920x1080
- AAC 48 kHz si usas audio embebido
- Keyframe interval 1 o 2 segundos
