# Hyperframes Composition Brief: GitHub Star Search

## Objective
Create a short launch-style brag video for GitHub Star Search, narrated (Kokoro `af_heart`).

## Output
- Composition directory: `brag-output/composition/`
- Rendered video: `brag-output/brag.mp4`
- Format: landscape — 1920x1080
- Duration: 24.5s (flexed to narration)

## Source Material
- Project root: repo root (Blazor Server app)
- Primary files read: `Components/Pages/Home.razor`, `Components/Layout/MainLayout.razor`, `Components/App.razor`,
  `wwwroot/css/spacey.css`, `wwwroot/js/spacey.js`, `README.md`, `wwwroot/icon-512.png`
- Product name: GitHub Star Search
- Tagline / strongest claim: "Did you star over 500 repositories and now can't find that awesome library you bookmarked last week?"
- Key UI to recreate: username card + Probe Stars button, search card + highlighted repo result cards, cleanup bar + obsolete cards
- Copy that must appear verbatim:
  - Enter your GitHub Username
  - Probe Stars 🌟 / Probing the universe…
  - Stars indexed — ready to explore! 🚀
  - Search Starred Repositories
  - Suggest cleanup 🧹 / Scanning orbit…
  - 📦 Archived by owner / 💤 No commits in 6 years
  - Open & unstar

## Creative Direction
- Tone preset: default
- Creative direction: space-mission launch for your bookmark graveyard
- Interpretation: playful and clean. The space metaphor comes from the app's own copy. Motion is energetic and every line holds long enough to read.
- Angle: mission control for your bookmark graveyard. The video shows the working app: probe, search, clean up.
- Hook: fast-scrolling star list + counter ★ 512 + "Last week's bookmark: somewhere in here."
- Outro / punchline: logo, "GitHub Star Search", "Find your stars again.", gss.janskala.cz
- Avoid: generic SaaS language, abstract filler, redesigning the app's look

## Visual Identity
- Background: #08080f with nebula glows (#7e6fff, #4a86ff, #c03bff, #3dcb6c) and a twinkling starfield
- Text: #c8c6d8 / titles #e0deef
- Accent: #7e6fff (+ #4a86ff blue, #ffb04a amber, #3dcb6c green)
- Display/body font: Space Grotesk (local @font-face); data font: Space Mono
- Visual references: glass cards, gradient Probe button with glow, purple mark highlights, amber obsolete streak, logo PNG

## Storyboard
Creative contract: `brag-output/brag-plan.md`.
1. Hook — 0–4.7s — star list whip-scroll, ★ 512 counter, "Last week's bookmark: somewhere in here."
2. Reveal — 4.7–6.6s — warp streaks, logo (beat-lock 4.91s), wordmark
3. Probe — 6.6–10.7s — type octocat, click, spinner, particle warp, success snackbar
4. Search — 10.7–16.6s — type "markdown", 4 highlighted cards, "-python" kicks one out, 4→3
5. Cleanup — 16.6–20.6s — Suggest cleanup click, Scanning orbit…, two obsolete cards dim
6. Outro — 20.6–24.5s — logo + name + "Find your stars again." (≈22.37s cue) + URL

## Audio
- Audio role: warm bed under narration + motion-matched UI accents
- Narration: `assets/vo/vo1..6.wav`, starting at 0.30, 4.85, 6.75, 10.90, 16.75, 20.85
- Music: `happy-beats-business-moves-vol-12-by-ende-dot-app.mp3`. A volume lane ducks it to ~0.13 under each VO line and lifts it to ~0.30 in the gaps. It fades in at the start and out at the end.
- Music cue guidance: `<skill-dir>/assets/music/cues/happy-beats-business-moves-vol-12-by-ende-dot-app.music-cues.json`. Locks: 4.91 (logo), 22.37 (tagline). Grid for the cascade: 12.02 / 12.55 / 13.11 / 13.64.
- Audio-reactive treatment: subtle. Bass/RMS drives the nebula glow and the logo halo.
- SFX: keyboard ticks (sparse), UI clicks, a soft impact on reveals, a bell on success/outro, card slides for the cascade. Low-HF-risk picks from `sfx-analysis.md`, at 0.45–0.7.
