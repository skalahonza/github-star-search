// ── Starfield + shooting stars + mouse parallax ──
(function () {
  const canvas = document.getElementById('starfield');
  if (!canvas) return;
  const ctx = canvas.getContext('2d');
  let W, H, stars = [], shooters = [];
  let mouse = { x: 0, y: 0 };

  function resize() {
    W = canvas.width  = window.innerWidth;
    H = canvas.height = window.innerHeight;
  }
  function initStars() {
    stars = Array.from({ length: 220 }, () => ({
      x: Math.random() * W, y: Math.random() * H,
      r: Math.random() * 1.6 + 0.2,
      speed: Math.random() * 0.012 + 0.003,
      phase: Math.random() * Math.PI * 2,
      parallax: Math.random() * 0.018 + 0.002,
      baseX: 0, baseY: 0,
    }));
    stars.forEach(s => { s.baseX = s.x; s.baseY = s.y; });
  }

  function spawnShooter() {
    shooters.push({
      x: Math.random() * W * 1.2 - W * 0.1,
      y: Math.random() * H * 0.4,
      len: Math.random() * 180 + 80,
      speed: Math.random() * 14 + 8,
      angle: Math.PI / 4 + (Math.random() - 0.5) * 0.4,
      life: 1, decay: Math.random() * 0.02 + 0.015,
    });
  }

  setInterval(spawnShooter, 2400);
  setTimeout(spawnShooter, 600);

  document.addEventListener('mousemove', e => {
    mouse.x = e.clientX - W / 2;
    mouse.y = e.clientY - H / 2;
  });

  function draw(t) {
    ctx.clearRect(0, 0, W, H);

    stars.forEach(s => {
      const alpha = 0.35 + 0.65 * (0.5 + 0.5 * Math.sin(t * s.speed + s.phase));
      const px = s.baseX + mouse.x * s.parallax;
      const py = s.baseY + mouse.y * s.parallax;
      ctx.beginPath();
      ctx.arc(px, py, s.r, 0, Math.PI * 2);
      ctx.fillStyle = `rgba(190,185,255,${alpha})`;
      ctx.fill();
    });

    shooters = shooters.filter(s => s.life > 0);
    shooters.forEach(s => {
      const dx = Math.cos(s.angle) * s.speed;
      const dy = Math.sin(s.angle) * s.speed;
      const grad = ctx.createLinearGradient(s.x, s.y,
        s.x - Math.cos(s.angle) * s.len, s.y - Math.sin(s.angle) * s.len);
      grad.addColorStop(0,   `rgba(200,195,255,${s.life * 0.9})`);
      grad.addColorStop(0.3, `rgba(126,111,255,${s.life * 0.5})`);
      grad.addColorStop(1,   'rgba(126,111,255,0)');
      ctx.beginPath();
      ctx.moveTo(s.x, s.y);
      ctx.lineTo(s.x - Math.cos(s.angle) * s.len, s.y - Math.sin(s.angle) * s.len);
      ctx.strokeStyle = grad;
      ctx.lineWidth = 1.5;
      ctx.stroke();
      s.x += dx; s.y += dy;
      s.life -= s.decay;
    });

    requestAnimationFrame(draw);
  }

  resize(); initStars(); requestAnimationFrame(draw);
  window.addEventListener('resize', () => { resize(); initStars(); });
})();

// ── Particle burst system (exposed for Blazor JS interop) ──
window.ParticleBurst = (function () {
  const canvas = document.getElementById('particles-canvas');
  if (!canvas) return { burst: () => {}, warpBurst: () => {}, burstAt: () => {} };
  const ctx = canvas.getContext('2d');
  let particles = [];
  let animating = false;

  function resize() { canvas.width = window.innerWidth; canvas.height = window.innerHeight; }
  resize(); window.addEventListener('resize', resize);

  function burst(x, y, color, count) {
    count = count || 28;
    for (let i = 0; i < count; i++) {
      const angle = (Math.PI * 2 * i) / count + Math.random() * 0.4;
      const speed = Math.random() * 5 + 2;
      const size  = Math.random() * 4 + 1.5;
      particles.push({
        x, y, vx: Math.cos(angle) * speed, vy: Math.sin(angle) * speed - Math.random() * 3,
        size, color: color || '#7e6fff', life: 1, decay: Math.random() * 0.025 + 0.015,
        gravity: 0.12,
      });
    }
    if (!animating) { animating = true; draw(); }
  }

  function warpBurst(count) {
    const W = canvas.width, H = canvas.height;
    count = count || 60;
    for (let i = 0; i < count; i++) {
      const angle  = Math.random() * Math.PI * 2;
      const radius = Math.random() * Math.max(W, H) * 0.6;
      const cx = W / 2, cy = H / 2;
      const sx = cx + Math.cos(angle) * radius * 0.1;
      const sy = cy + Math.sin(angle) * radius * 0.1;
      const speed = Math.random() * 14 + 6;
      const hue = Math.random() > 0.5 ? '#7e6fff' : '#4a86ff';
      particles.push({
        x: sx, y: sy,
        vx: Math.cos(angle) * speed, vy: Math.sin(angle) * speed,
        size: Math.random() * 3 + 1, color: hue,
        life: 1, decay: Math.random() * 0.02 + 0.012, gravity: 0,
      });
    }
    if (!animating) { animating = true; draw(); }
  }

  // Burst centered on a DOM element (Blazor passes ElementReference)
  function burstAt(element, color, count) {
    if (!element || !element.getBoundingClientRect) return;
    const r = element.getBoundingClientRect();
    burst(r.left + r.width / 2, r.top + r.height / 2, color, count);
  }

  function draw() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    particles = particles.filter(p => p.life > 0);
    particles.forEach(p => {
      ctx.globalAlpha = p.life * p.life;
      ctx.beginPath();
      ctx.arc(p.x, p.y, p.size * p.life, 0, Math.PI * 2);
      ctx.fillStyle = p.color;
      ctx.fill();
      p.x += p.vx; p.y += p.vy;
      p.vy += p.gravity; p.vx *= 0.97;
      p.life -= p.decay;
    });
    ctx.globalAlpha = 1;
    if (particles.length > 0) requestAnimationFrame(draw);
    else animating = false;
  }

  return { burst, warpBurst, burstAt };
})();
