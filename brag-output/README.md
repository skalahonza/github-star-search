# Launch video

The source for the ~25s launch video shown on the welcome screen (`wwwroot/video/brag.mp4`).
It was made with the `/brag` Claude Code skill and [Hyperframes](https://www.npmjs.com/package/hyperframes).

- `brag-plan.md`: angle, storyboard, narration script
- `composition-brief.md`: the brief handed to Hyperframes
- `composition/`: the Hyperframes project (`index.html` plus assets; narration is Kokoro `af_heart`)
- `share-copy.txt`: the caption for social posts
- `brag.jpg`: the poster frame (hook at 3.4s)

## Re-rendering

The music bed (`happy-beats-business-moves-vol-12-by-ende-dot-app.mp3` from ende.app) isn't committed.
Copy it from the `/brag` plugin's `assets/music/` into `composition/assets/music/` first.

```bash
cd brag-output/composition
npx hyperframes check
npx hyperframes render --quality delivery --output ../brag.mp4
cd ..
# bake the poster as frame 0 and normalize loudness to -16 LUFS
ffmpeg -ss 3.4 -i brag.mp4 -frames:v 1 -q:v 2 brag.jpg
ffmpeg -i brag.mp4 -i brag.jpg -filter_complex "[0:v][1:v]overlay=0:0:enable='eq(n,0)'[v]" \
  -map "[v]" -map 0:a -c:v libx264 -crf 18 -preset slow -pix_fmt yuv420p \
  -af "loudnorm=I=-16:TP=-1.5:LRA=11" -ar 48000 -c:a aac -b:a 192k -movflags +faststart brag.final.mp4
# lighter web encode for the app
ffmpeg -i brag.final.mp4 -c:v libx264 -crf 23 -preset slow -pix_fmt yuv420p -c:a copy -movflags +faststart ../wwwroot/video/brag.mp4
cp brag.jpg ../wwwroot/video/brag.jpg
```
