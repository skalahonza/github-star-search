# Brag Plan: GitHub Star Search

## What is this app?
A Blazor + Meilisearch web app that indexes every public repo you've starred on GitHub and gives you
instant, full-text search across names, descriptions and READMEs. It can also suggest which of your stars
went dark and are worth cleaning up.

## The angle
**Mission control for your bookmark graveyard.** Everyone has hundreds of GitHub stars and zero ability
to find any of them. The app already talks like a space mission ("Probe Stars 🌟", "Probing the
universe…", "Scanning orbit…", stars that "went dark years ago"), so the video plays that straight:
your stars really are a galaxy, and this is the probe that maps it. The payoff is the product itself
working: type a username, fire the probe, search instantly, then clean up the dead stars.

## Hook (first 2-3 seconds)
A GitHub-style "starred repositories" list whips past, too fast to read, while a big counter ticks up to
**★ 512**. The narrator names the problem out loud. The second text beat, "Last week's bookmark: somewhere in
here.", lands while the list is still scrolling. You can see the problem before you hear the product name.

## Key moments (the middle)
- **The probe fires.** "octocat" gets typed into the real username card, the cursor hits the glowing
  gradient **Probe Stars 🌟** button, it flips to a spinner reading **Probing the universe…**, and
  particles warp out from the button (the app's own `warpBurst`). The green snackbar
  **"Stars indexed — ready to explore! 🚀"** slides in.
- **Search as you type.** Type `markdown` into the search card. Four repo cards cascade in with the app's
  purple `<mark>` highlights on the title, the description and the README line. Then ` -python` is typed, the
  Python result gets kicked out and the count flips **4 results → 3 results**.
- **The cleanup.** Click **Suggest cleanup 🧹**, see **Scanning orbit…**, and obsolete-star cards arrive with the
  amber streak and the real badges **📦 Archived by owner** and **💤 No commits in 6 years**. The cards dim as
  they "go dark".

## Outro / punchline
The logo (star + magnifier + orbit) and **GitHub Star Search**, then **Find your stars again.** with
`gss.janskala.cz` and the footer's "★ Star on GitHub" pill. The last joke: the one repo you should star is this one.

## User flow worth showing
1. Entry: type a GitHub username → click **Probe Stars 🌟** → spinner "Probing the universe…" →
   success snackbar.
2. Key action: type into **Search Starred Repositories** → highlighted results appear as you type → the `-`
   operator excludes a word.
3. Result/bonus: **Suggest cleanup 🧹** → obsolete stars with archived/stale badges.

## Tone
- Preset: `default`
- Creative direction: space-mission launch for your bookmark graveyard
- Interpretation: warm, playful, clean; the space metaphor comes from the app's own copy rather than being
  pasted on. Motion is energetic (warp streaks, cascades), text holds long enough to read, and nothing is chaotic.

## Format: landscape — 1920x1080
## Duration: ~24.5s (set by narration; 6 scenes)

## Visual identity (from the project)
- Background: `#08080f` (+ nebula blobs `#7e6fff`, `#4a86ff`, `#c03bff`, `#3dcb6c` at low opacity, twinkling starfield)
- Accent: `#7e6fff` (primary), `#4a86ff` (info/blue), `#ffb04a` (cleanup amber), `#3dcb6c` (success green)
- Text: `#c8c6d8` (primary), `#e0deef` (titles), `#8886a0` / `#a09fba` (secondary)
- Display font: Space Grotesk (700)
- Body/data font: Space Grotesk; Space Mono for counts, dates and badges
- Strongest visual element: glassy cards (`rgba(22,22,42,0.7)`, 16px radius, purple hairline borders), the
  glowing gradient button `linear-gradient(135deg,#7e6fff,#5b8aff)`, purple `mark` highlights, the logo.

## Share copy (draft)
Starred 500+ repos and can't find any of them? I built GitHub Star Search: full-text search across names,
descriptions and READMEs of everything you've ever starred, plus cleanup for the stars that went dark. ⭐🔭

## Audio direction
- Role: warm bed under narration + motion-matched UI accents
- Music: `happy-beats-business-moves-vol-12-by-ende-dot-app.mp3` (steady, clean, sits under a voice)
- Music treatment: fade in over 0.6s, bed ~0.30, ducked to ~0.13 while the narrator speaks, lifting to 0.30 in
  the gaps and on the outro, fading out over the last ~1.2s.
- Music cue guidance: preset `assets/music/cues/happy-beats-business-moves-vol-12-by-ende-dot-app.music-cues.json`
  (109.96 BPM). Strong locks: logo reveal on the **4.91s** beat; outro tagline near **22.37s** (strong cue).
  Beat grid for the search-card cascade: 12.02 / 12.55 / 13.11 / 13.64. Reveal fast, then hold all four.
- Audio-reactive treatment: subtle. Music RMS/bass makes the nebula glow and the logo halo breathe. No
  visualizer bars.
- SFX posture: moderate and motion-matched. Keypress ticks on typed text, a click on the probe button, a
  soft whoosh-y impact on the warp, a bell on "indexed" success, card sounds for the cascade, a soft
  impact for the logo.
- Audio-coupled moments: typing (username, query, `-python`), button clicks, card cascade, snackbar success, logo.
- Restraint rule: SFX never cover narration consonants. Keep them at 0.5-0.7, skip ticks where copy is dense.

## Voiceover script
Kokoro `af_heart` at 1.08x, one clip per scene (`composition/assets/vo/vo1..6.wav`):

1. "You've starred five hundred repos. And you still can't find the one from last week." (4.27s)
2. "Meet GitHub Star Search." (1.47s)
3. "Type your username, fire the probe, and every star gets indexed." (3.67s)
4. "Then just search. Names, descriptions, even READMEs. Put a minus on a word to kick it out." (5.40s)
5. "And the stars that went dark years ago? It'll help you clean those up, too." (3.56s)
6. "GitHub Star Search. Find your stars again." (2.56s)

The narration is written to complement what's on screen. The screen shows the exact counts, UI copy and badges.
The voice gives the intent and the joke.

## Storyboard

### Scene 1 — Hook: the star pile — 0.0–4.7s (4.7s)
A tilted GitHub-style list of starred repo rows (★, owner/name, a language dot) scrolls fast and motion-blurred.
The big counter **★ 512** ("starred repositories") ticks up in Space Mono. At ~2.4s a pill appears: "Last
week's bookmark: somewhere in here." and holds.
Sequential/interaction: yes. The counter ticks up and the list scrolls.
Audio intent: curiosity with a slight overwhelm. The music fades in.
Audio-coupled idea: soft ticks while the counter climbs (sparse).
Transition mood: dramatic. The rows get pulled into warp streaks → Scene 2

### Scene 2 — Reveal — 4.7–6.6s (1.9s)
Warp streaks radiate from the center. The logo lands (beat-locked 4.91s) with a purple halo, then
**GitHub Star Search** and a small mono label "full-text search for your GitHub stars".
Sequential/interaction: none
Audio intent: arrival. One soft impact.
Transition mood: clean push-in → Scene 3

### Scene 3 — Probe the stars — 6.6–10.7s (4.1s)
The real username card at video scale: "Enter your GitHub Username", the input, and the gradient **Probe Stars 🌟** button.
"octocat" types in. The cursor clicks at ~"fire the probe". The button flips to a spinner with **Probing the universe…**, and
particles warp out. Then the snackbar **Stars indexed — ready to explore! 🚀** appears, alongside a mono readout "512 repos indexed".
Sequential/interaction: yes. Typing, click, spinner, success.
Audio-coupled idea: key ticks, a click, a success bell.
Transition mood: clean slide up → Scene 4

### Scene 4 — Search as you type — 10.7–16.6s (5.9s)
The search card ("Search Starred Repositories") types `markdown`. The pill **4 results** matching "markdown" appears, and a
2×2 grid of repo cards cascades in: `xoofx/markdig`, `markdown-it/markdown-it`, `yuin/goldmark`,
`lepture/mistune`. The matches are highlighted in title, description and README. During "names,
descriptions, even READMEs" the highlights pulse in order: title → description → README. Then ` -python` is typed and the
mistune card is kicked out. The pill flips to **3 results**.
Sequential/interaction: yes. Typing, a 4-card cascade (fast in, then all held ≥2.5s), and an exclusion.
Audio-coupled idea: key ticks, card slides, one soft "drop" as the card leaves.
Transition mood: clean wipe → Scene 5

### Scene 5 — Clean up dead stars — 16.6–20.6s (4.0s)
The cleanup bar: 🧹 **Cleanup suggestions**, "Silent for 2+ years", and **Suggest cleanup 🧹** gets clicked. It shows
**Scanning orbit…**, then "37 obsolete stars" and two amber obsolete cards: `request/request` (📦 Archived by owner ·
💤 No commits in 6 years) and `atom/atom` (📦 Archived by owner · 💤 No commits in 3 years) with **Open & unstar**. The cards
dim like dying stars as the voice says "went dark".
Sequential/interaction: yes. Click, spinner, two cards.
Audio-coupled idea: a click, card place sounds.
Transition mood: soft crossfade → Scene 6

### Scene 6 — Outro — 20.6–24.5s (3.9s)
The logo with its orbiting ring, **GitHub Star Search**, then **Find your stars again.** (locked near the 22.37s strong cue).
Below: `gss.janskala.cz` and the "★ Star on GitHub" pill. Hold. The music rings out.
Audio-coupled idea: a logo bell at the tagline.

**Music mood for this video:** upbeat, clean
**Audio summary:** a warm bed fades in under a friendly narrator, ducks for each line, gets small UI ticks and clicks
for every interaction, and swells to a bell on the final tagline before fading out.

## Data notes
Username "octocat" is GitHub's public mascot account, used as a stand-in. The repos shown are real public
open-source projects, with lightly paraphrased descriptions. "512" and "37" are illustrative counts. No real
user data or secrets appear.
