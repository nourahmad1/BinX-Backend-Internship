<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<title>Day 1 — ASP.NET Core Identity & User Registration</title>
<link rel="preconnect" href="https://fonts.googleapis.com">
<link href="https://fonts.googleapis.com/css2?family=Caveat:wght@500;700&family=Lora:ital,wght@0,400;0,500;0,600;1,400&family=JetBrains+Mono:wght@400;500;700&display=swap" rel="stylesheet">
<style>
  :root{
    --paper: #eef1ec;
    --paper-line: #c9d6da;
    --margin-red: #c85a4a;
    --ink: #1d2b3a;
    --ink-soft: #3d4f60;
    --stamp-red: #a8352a;
    --teal: #2f6f63;
    --teal-soft: #dce9e5;
    --tape: #edd57a;
    --hole: #d7ddd0;
    --paper-shadow: rgba(29,43,58,0.18);
  }

  *{box-sizing:border-box;}

  html,body{
    margin:0;
    padding:0;
    background:#d8dcd3;
    font-family:'Lora', Georgia, serif;
    color:var(--ink);
  }

  .desk{
    padding:48px 16px;
    display:flex;
    justify-content:center;
  }

  .page{
    position:relative;
    max-width:820px;
    width:100%;
    background:
      repeating-linear-gradient(
        var(--paper),
        var(--paper) 33px,
        var(--paper-line) 34px
      );
    background-color:var(--paper);
    box-shadow: 0 30px 60px -20px var(--paper-shadow), 0 2px 6px rgba(0,0,0,0.08);
    border-radius:3px;
    padding:70px 60px 60px 90px;
  }

  .margin-rule{
    position:absolute;
    top:0;
    bottom:0;
    left:64px;
    width:2px;
    background:var(--margin-red);
    opacity:0.55;
  }

  .holes{
    position:absolute;
    left:26px;
    top:60px;
    display:flex;
    flex-direction:column;
    gap:110px;
  }
  .holes span{
    width:16px;
    height:16px;
    border-radius:50%;
    background:var(--hole);
    box-shadow: inset 0 2px 3px rgba(0,0,0,0.25);
    display:block;
  }

  .tape{
    position:absolute;
    width:90px;
    height:30px;
    background: linear-gradient(180deg, var(--tape), #e4c65e);
    opacity:0.85;
    box-shadow: 0 2px 4px rgba(0,0,0,0.15);
  }
  .tape.tl{ top:-14px; left:70px; transform:rotate(-6deg); }
  .tape.tr{ top:-14px; right:60px; transform:rotate(5deg); }

  header{
    margin-bottom:36px;
  }

  .kicker{
    font-family:'JetBrains Mono', monospace;
    font-size:12px;
    letter-spacing:0.14em;
    text-transform:uppercase;
    color:var(--teal);
    margin:0 0 10px 0;
  }

  h1{
    font-family:'Caveat', cursive;
    font-weight:700;
    font-size:52px;
    line-height:1.05;
    margin:0 0 6px 0;
    color:var(--ink);
  }

  .subtitle{
    font-style:italic;
    color:var(--ink-soft);
    font-size:17px;
    margin:0 0 18px 0;
  }

  .meta-row{
    display:flex;
    gap:22px;
    flex-wrap:wrap;
    font-family:'JetBrains Mono', monospace;
    font-size:12.5px;
    color:var(--ink-soft);
    border-top:1px dashed #b9c4bf;
    padding-top:12px;
  }
  .meta-row b{ color:var(--ink); }

  h2{
    font-family:'Caveat', cursive;
    font-weight:700;
    font-size:32px;
    color:var(--teal);
    margin:44px 0 6px 0;
    display:flex;
    align-items:baseline;
    gap:10px;
  }
  h2 .tag{
    font-family:'JetBrains Mono', monospace;
    font-size:12px;
    font-weight:500;
    color:#fff;
    background:var(--teal);
    padding:3px 8px;
    border-radius:3px;
    letter-spacing:0.03em;
  }

  p{
    font-size:16px;
    line-height:1.7;
    margin:0 0 14px 0;
    color:var(--ink);
  }

  .entry{
    display:flex;
    gap:16px;
    margin:18px 0 22px 0;
  }
  .entry .num{
    flex:0 0 auto;
    font-family:'JetBrains Mono', monospace;
    font-weight:700;
    font-size:13px;
    color:#fff;
    background:var(--ink);
    width:26px;
    height:26px;
    border-radius:50%;
    display:flex;
    align-items:center;
    justify-content:center;
    margin-top:3px;
  }
  .entry .body h3{
    font-family:'Lora', serif;
    font-weight:600;
    font-size:17px;
    margin:0 0 4px 0;
    color:var(--ink);
  }
  .entry .body p{ margin:0; }

  .codecard{
    background:#182226;
    color:#dce7e4;
    border-radius:6px;
    padding:18px 20px;
    font-family:'JetBrains Mono', monospace;
    font-size:13px;
    line-height:1.65;
    margin:14px 0 20px 0;
    overflow-x:auto;
    white-space:pre-wrap;
    box-shadow: 0 6px 14px rgba(0,0,0,0.18);
    position:relative;
  }
  .codecard::before{
    content:"snippet";
    position:absolute;
    top:-10px;
    left:14px;
    background:var(--teal);
    color:#fff;
    font-size:10px;
    letter-spacing:0.08em;
    text-transform:uppercase;
    padding:2px 8px;
    border-radius:3px;
  }
  .codecard .kw{ color:#e8b96a; }
  .codecard .fn{ color:#7fc9c0; }
  .codecard .ty{ color:#a7c980; }

  .sticky{
    background:var(--tape);
    background:linear-gradient(180deg,#f3dd85,#ecc94b);
    padding:16px 20px;
    margin:24px 0;
    max-width:520px;
    box-shadow: 3px 5px 10px rgba(0,0,0,0.15);
    transform:rotate(-1.2deg);
    font-family:'Lora', serif;
    font-size:14.5px;
    line-height:1.55;
    color:#3a2f10;
    position:relative;
  }
  .sticky b{ display:block; font-family:'Caveat', cursive; font-size:19px; margin-bottom:4px; color:#5c4a10;}

  .checklist{
    list-style:none;
    margin:14px 0 22px 0;
    padding:0;
  }
  .checklist li{
    display:flex;
    align-items:flex-start;
    gap:10px;
    padding:8px 0;
    border-bottom:1px dashed #c6cec5;
    font-size:15px;
    line-height:1.55;
  }
  .checklist li:last-child{ border-bottom:none; }
  .checklist .box{
    flex:0 0 auto;
    width:19px;
    height:19px;
    margin-top:2px;
    border:2px solid var(--ink);
    border-radius:3px;
    position:relative;
  }
  .checklist .box::after{
    content:"";
    position:absolute;
    left:3px; top:-1px;
    width:8px; height:12px;
    border-right:2.5px solid var(--stamp-red);
    border-bottom:2.5px solid var(--stamp-red);
    transform:rotate(35deg);
  }

  .tools-line{
    font-family:'JetBrains Mono', monospace;
    font-size:13px;
    color:var(--ink-soft);
    padding:10px 14px;
    background:var(--teal-soft);
    border-radius:5px;
    display:inline-block;
    margin-top:4px;
  }

  .testgrid{
    display:grid;
    grid-template-columns:1fr 1fr;
    gap:22px;
    margin:22px 0 10px 0;
  }
  @media (max-width:640px){ .testgrid{ grid-template-columns:1fr; } }

  .testcard{
    border:1.5px solid #c7cfc4;
    border-radius:6px;
    padding:18px 96px 20px 18px;
    background:rgba(255,255,255,0.5);
    position:relative;
    overflow:hidden;
  }
  .testcard h4{
    font-family:'JetBrains Mono', monospace;
    font-size:12px;
    text-transform:uppercase;
    letter-spacing:0.06em;
    color:var(--ink-soft);
    margin:0 0 8px 0;
  }
  .testcard .req{
    font-family:'JetBrains Mono', monospace;
    font-size:13px;
    color:var(--ink);
    background:#fff;
    border:1px solid #dfe4dc;
    border-radius:4px;
    padding:8px 10px;
    margin-bottom:10px;
    word-break:break-word;
  }
  .testcard p{ font-size:14.5px; margin:0; }

  .stamp{
    position:absolute;
    top:16px;
    right:12px;
    width:78px;
    font-family:'JetBrains Mono', monospace;
    font-weight:700;
    font-size:11.5px;
    line-height:1.25;
    text-align:center;
    letter-spacing:0.04em;
    color:var(--stamp-red);
    border:3px solid var(--stamp-red);
    border-radius:6px;
    padding:6px 4px;
    transform:rotate(9deg);
    opacity:0.85;
    background:rgba(255,255,255,0.5);
  }

  .tree{
    font-family:'JetBrains Mono', monospace;
    font-size:13.5px;
    line-height:1.85;
    background:#fff;
    border:1px solid #dfe4dc;
    border-radius:6px;
    padding:16px 20px;
    margin:16px 0 24px 0;
    color:var(--ink);
    white-space:pre;
    overflow-x:auto;
  }
  .tree .folder{ color:var(--teal); font-weight:700; }

  footer{
    margin-top:50px;
    padding-top:18px;
    border-top:2px solid var(--ink);
    display:flex;
    justify-content:space-between;
    align-items:center;
    flex-wrap:wrap;
    gap:12px;
  }
  footer .filebadge{
    font-family:'JetBrains Mono', monospace;
    font-size:12.5px;
    background:var(--ink);
    color:#fff;
    padding:7px 12px;
    border-radius:4px;
  }
  footer .signoff{
    font-family:'Caveat', cursive;
    font-size:24px;
    color:var(--ink-soft);
  }
</style>
</head>
<body>
<div class="desk">
  <div class="page">
    <div class="margin-rule"></div>
    <div class="holes"><span></span><span></span><span></span><span></span></div>
    <div class="tape tl"></div>
    <div class="tape tr"></div>

    <header>
      <p class="kicker">Learning Log · Backend Track</p>
      <h1>Day 1 — ASP.NET Core Identity &amp; User Registration</h1>
      <p class="subtitle">Field notes from the day I finally stopped being scared of auth.</p>
      <div class="meta-row">
        <span><b>Time spent:</b> 8 hours</span>
        <span><b>Stack:</b> ASP.NET Core · EF Core · Postman</span>
        <span><b>Full report:</b> Postman_API_Testing_Report.docx</span>
      </div>
    </header>

    <p>Today's focus was authentication — specifically, not building it myself. ASP.NET Core Identity handles the parts of a login system that are easiest to get wrong, and the goal for the day was to understand what it gives you for free, wire it into a real project, and actually test that it works.</p>

    <h2><span class="tag">learned</span> What Identity actually gives you</h2>

    <div class="entry">
      <div class="num">1</div>
      <div class="body">
        <h3>It's a complete membership system, not just a users table</h3>
        <p>Storage, password hashing, roles, account confirmation — all of it ships out of the box, sitting on top of Entity Framework Core. The real value isn't convenience, it's that this code has already been picked apart by Microsoft and the entire .NET community. Rolling your own version means reinventing something security-critical, badly, alone.</p>
      </div>
    </div>

    <div class="entry">
      <div class="num">2</div>
      <div class="body">
        <h3>Wiring it up is mostly one inheritance change</h3>
        <p>Extend the existing <code>DbContext</code> to inherit from <code>IdentityDbContext</code> and the full Identity schema — Users, Roles, UserRoles, and a few supporting tables — comes along for the ride. One migration later, it's sitting next to whatever tables were already there from last week.</p>
      </div>
    </div>

    <div class="codecard"><span class="kw">public class</span> <span class="ty">AppDbContext</span> : <span class="ty">IdentityDbContext</span>
{
    <span class="kw">public</span> DbSet&lt;Order&gt; Orders =&gt; Set&lt;Order&gt;();
}

builder.Services.<span class="fn">AddIdentity</span>&lt;IdentityUser, IdentityRole&gt;()
    .<span class="fn">AddEntityFrameworkStores</span>&lt;AppDbContext&gt;();</div>

    <div class="entry">
      <div class="num">3</div>
      <div class="body">
        <h3>Registration is mostly plumbing, not logic</h3>
        <p><code>UserManager.CreateAsync</code> does the actual work — hashing the password, saving the user — in one call. Writing the endpoint is really about validating what comes in and turning the result back into the right response: success, or a clear list of what went wrong.</p>
      </div>
    </div>

    <div class="entry">
      <div class="num">4</div>
      <div class="body">
        <h3>The hashing story is more thoughtful than I expected</h3>
        <p>Identity hashes passwords with PBKDF2 by default — deliberately slow, and salted per user, so a leaked database can't be cracked with a rainbow table in one pass. Two users with the same password end up with completely different stored hashes.</p>
      </div>
    </div>

    <div class="sticky">
      <b>note to self</b>
      Don't ever write custom password hashing. Not "don't unless you have a good reason" — just don't. Identity's version has been battle-tested by a much bigger crowd than will ever review my code.
    </div>

    <h2><span class="tag">built</span> The hands-on lab</h2>
    <p>Here's what actually got done today, in order:</p>
    <ul class="checklist">
      <li><span class="box"></span> Added the Identity NuGet packages and extended the DbContext to inherit from <code>IdentityDbContext</code>.</li>
      <li><span class="box"></span> Ran a migration to add the Identity schema to the database, then applied it.</li>
      <li><span class="box"></span> Registered Identity services in <code>Program.cs</code> with <code>IdentityUser</code> and <code>IdentityRole</code>.</li>
      <li><span class="box"></span> Implemented a registration endpoint using <code>UserManager.CreateAsync</code>, with meaningful errors for bad input.</li>
      <li><span class="box"></span> Tested registration in Postman — once with a valid request, once with a deliberately weak password.</li>
    </ul>
    <span class="tools-line">tools: ASP.NET Core Identity · Entity Framework Core · Postman</span>

    <h2><span class="tag">tested</span> Putting the endpoint through Postman</h2>
    <p>With the endpoint built, the last step was proving it actually behaves — both when everything is correct and when it isn't.</p>

    <div class="testgrid">
      <div class="testcard">
        <div class="stamp">200 OK</div>
        <h4>Register – Valid User</h4>
        <div class="req">POST /api/Auth/register</div>
        <p>Sent a valid username, email, and a strong password. The user was created successfully through <code>UserManager.CreateAsync</code> — exactly what should happen.</p>
      </div>
      <div class="testcard">
        <div class="stamp">400 Bad Request</div>
        <h4>Register – Weak Password</h4>
        <div class="req">POST /api/Auth/register</div>
        <p>Same endpoint, same shape of request, but this time with a deliberately weak password. Identity rejected it and returned clear validation errors, just as it should.</p>
      </div>
    </div>

    <p>Both requests live in one Postman collection, organized like this:</p>
    <div class="tree"><span class="folder">Task Tracker API - Week 4 Day 1</span>
└── <span class="folder">Authentication</span>
    ├── Register - Valid User
    └── Register - Weak Password</div>

    <p>Between the two, both sides of registration are covered: a real user getting created, and a bad password getting caught before it ever reaches the database.</p>

    <footer>
      <span class="filebadge">📄 Postman_API_Testing_Report.docx — full test details &amp; screenshots</span>
      <span class="signoff">— end of Day 1</span>
    </footer>
  </div>
</div>
</body>
</html>
