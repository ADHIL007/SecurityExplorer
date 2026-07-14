// ─────────────────────────────────────────────────────────────────────────────
// State
// ─────────────────────────────────────────────────────────────────────────────
let testResults = [];

// ─────────────────────────────────────────────────────────────────────────────
// Bootstrap
// ─────────────────────────────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
  renderDashboardShell();
  runTests();
});

// ─────────────────────────────────────────────────────────────────────────────
// Shell & navigation
// ─────────────────────────────────────────────────────────────────────────────
function renderDashboardShell() {
  const appRoot = document.getElementById('appRoot');
  appRoot.innerHTML = `
    <div class="header">
      <h1 id="viewTitle">Dashboard</h1>
      <button class="btn-run" id="runTestsBtn" onclick="runTests()">Run All Tests</button>
    </div>
    <div id="contentArea">
      <p class="text-secondary">Click <strong>Run All Tests</strong> to begin.</p>
    </div>`;
}

function setActiveLink(linkId) {
  document.querySelectorAll('.sidebar-menu a').forEach(a => a.classList.remove('active'));
  const target = linkId
    ? document.getElementById(linkId)
    : document.querySelector('#mainMenu a');
  if (target) target.classList.add('active');
}

function loadDashboard() {
  renderDashboardShell();
  populateDashboardContent();
  setActiveLink(null);
}

// ─────────────────────────────────────────────────────────────────────────────
// Test execution
// ─────────────────────────────────────────────────────────────────────────────
async function runTests() {
  // Ensure the shell is present
  if (!document.getElementById('runTestsBtn')) renderDashboardShell();

  const btn     = document.getElementById('runTestsBtn');
  const content = document.getElementById('contentArea');

  if (btn) { btn.disabled = true; btn.textContent = 'Running…'; }
  if (content) content.innerHTML = '<p class="text-secondary">Running security tests…</p>';

  try {
    let basePath = window.location.pathname.replace(/\/(index\.html)?$/, '');
    const response = await fetch(`${basePath}/run`);
    if (!response.ok) throw new Error(`Server returned ${response.status}`);

    testResults = await response.json();

    updateSidebar();
    populateDashboardContent();

  } catch (err) {
    if (content) {
      content.innerHTML = '';
      const p = document.createElement('p');
      p.style.color = 'var(--status-fail)';
      p.textContent = `Error running tests: ${err.message}`;
      content.appendChild(p);
    }
    document.getElementById('pluginsMenu').innerHTML =
      '<li class="no-plugins">Could not load plugins</li>';
  } finally {
    if (btn) { btn.disabled = false; btn.textContent = 'Run All Tests'; }
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Sidebar
// ─────────────────────────────────────────────────────────────────────────────
function updateSidebar() {
  const menu = document.getElementById('pluginsMenu');
  menu.innerHTML = '';

  const withPage = testResults.filter(r => r.Page);
  if (withPage.length === 0) {
    menu.innerHTML = '<li class="no-plugins">No plugins loaded</li>';
    return;
  }

  testResults.forEach((result, index) => {
    if (!result.Page) return;
    const li = document.createElement('li');
    const a  = document.createElement('a');
    a.id        = `plugin-link-${index}`;
    a.textContent = result.Result?.Title || result.TestName || `Plugin ${index + 1}`;
    a.onclick   = () => loadPluginDetails(index);
    li.appendChild(a);
    menu.appendChild(li);
  });
}

// ─────────────────────────────────────────────────────────────────────────────
// Dashboard view
// ─────────────────────────────────────────────────────────────────────────────
function populateDashboardContent() {
  setActiveLink(null);

  const viewTitle = document.getElementById('viewTitle');
  if (viewTitle) viewTitle.textContent = 'Dashboard';

  const content = document.getElementById('contentArea');
  if (!content) return;
  content.innerHTML = '';

  if (testResults.length === 0) {
    content.innerHTML = '<p class="text-secondary">No results available.</p>';
    return;
  }

  const grid = document.createElement('div');
  grid.className = 'dashboard-grid';

  testResults.forEach(result => {
    grid.appendChild(
      result.Widget
        ? renderWidget(result)
        : renderDefaultCard(result)
    );
  });

  content.appendChild(grid);
}

/** Renders a structured plugin card from TestWidget + TestResult data. */
function renderWidget(result) {
  const w = result.Widget;
  const r = result.Result;

  const card = document.createElement('div');
  card.className = 'plugin-card';

  // ── Title row (name + status badge)
  const titleRow = document.createElement('div');
  titleRow.className = 'card-title-row';

  const title = document.createElement('p');
  title.className = 'card-title';
  title.textContent = r?.Title || result.TestName || 'Unnamed Test';

  titleRow.appendChild(title);
  if (r?.Status !== undefined) titleRow.appendChild(renderStatusBadge(r.Status));
  card.appendChild(titleRow);

  // ── Summary
  if (w.Summary) {
    const summary = document.createElement('p');
    summary.className = 'card-summary';
    summary.textContent = w.Summary;
    card.appendChild(summary);
  }

  // ── Metrics grid
  if (w.Metrics && Object.keys(w.Metrics).length > 0) {
    const metricsEl = document.createElement('div');
    metricsEl.className = 'metrics-grid';

    for (const [label, value] of Object.entries(w.Metrics)) {
      const block = document.createElement('div');
      block.className = 'metric-block';

      const valEl = document.createElement('span');
      valEl.className = 'metric-value';
      valEl.textContent = value;

      const lblEl = document.createElement('span');
      lblEl.className = 'metric-label';
      lblEl.textContent = label;

      block.appendChild(valEl);
      block.appendChild(lblEl);
      metricsEl.appendChild(block);
    }

    card.appendChild(metricsEl);
  }

  // ── Badge label (e.g. PROTECTED / UNPROTECTED)
  if (w.BadgeLabel) {
    const badgeRow = document.createElement('div');
    badgeRow.appendChild(renderBadgeRaw(w.BadgeLabel, r?.Status));
    card.appendChild(badgeRow);
  }

  return card;
}

/** Fallback card when no Widget data is provided. */
function renderDefaultCard(result) {
  const r = result.Result;
  const card = document.createElement('div');
  card.className = 'default-card';

  const titleRow = document.createElement('div');
  titleRow.style.cssText = 'display:flex;justify-content:space-between;align-items:center;margin-bottom:8px;';

  const h3 = document.createElement('h3');
  h3.textContent = r?.Title || result.TestName || 'Unknown Test';

  titleRow.appendChild(h3);
  if (r?.Status !== undefined) titleRow.appendChild(renderStatusBadge(r.Status));
  card.appendChild(titleRow);

  if (r?.Description) {
    const p = document.createElement('p');
    p.className = 'card-summary';
    p.textContent = r.Description;
    card.appendChild(p);
  }

  return card;
}

// ─────────────────────────────────────────────────────────────────────────────
// Detail page view
// ─────────────────────────────────────────────────────────────────────────────
function loadPluginDetails(index) {
  setActiveLink(`plugin-link-${index}`);
  const result = testResults[index];
  const page   = result.Page;
  const r      = result.Result;

  const appRoot = document.getElementById('appRoot');
  appRoot.innerHTML = '';

  // ── Page header
  const header = document.createElement('div');
  header.className = 'header';

  const h1 = document.createElement('h1');
  h1.id = 'viewTitle';
  h1.textContent = page?.Title || r?.Title || 'Plugin Details';

  const backBtn = document.createElement('button');
  backBtn.className = 'btn-run';
  backBtn.textContent = '← Back to Dashboard';
  backBtn.onclick = loadDashboard;

  header.appendChild(h1);
  header.appendChild(backBtn);
  appRoot.appendChild(header);

  if (!page) {
    // No detail page data — show raw JSON for debugging
    const card = document.createElement('div');
    card.className = 'section-card';
    card.innerHTML = '<p class="section-heading">Raw Output</p>';
    const pre = document.createElement('pre');
    pre.className = 'section-content is-code';
    pre.textContent = JSON.stringify(result, null, 2);
    card.appendChild(pre);
    appRoot.appendChild(card);
    return;
  }

  const detail = document.createElement('div');
  detail.className = 'detail-page';

  // ── Status summary card
  const summaryCard = document.createElement('div');
  summaryCard.className = 'section-card';
  const summaryRow = document.createElement('div');
  summaryRow.style.cssText = 'display:flex;align-items:center;gap:10px;flex-wrap:wrap;';
  summaryRow.appendChild(renderStatusBadge(r?.Status));
  if (r?.Type !== undefined) {
    const typeBadge = document.createElement('span');
    typeBadge.className = 'badge sev-info';
    typeBadge.textContent = statusTypeLabel(r.Type);
    summaryRow.appendChild(typeBadge);
  }
  summaryCard.appendChild(summaryRow);
  if (r?.Description) {
    const desc = document.createElement('p');
    desc.className = 'section-content';
    desc.style.marginTop = '10px';
    desc.textContent = r.Description;
    summaryCard.appendChild(desc);
  }
  detail.appendChild(summaryCard);

  // ── Sections
  (page.Sections || []).forEach(section => {
    const card = document.createElement('div');
    card.className = 'section-card';

    const heading = document.createElement('p');
    heading.className = 'section-heading';
    heading.textContent = section.Heading;

    const content = document.createElement('pre');
    content.className = section.IsCode ? 'section-content is-code' : 'section-content';
    if (!section.IsCode) content.style.whiteSpace = 'normal';
    content.textContent = section.Content;

    card.appendChild(heading);
    card.appendChild(content);
    detail.appendChild(card);
  });

  // ── Findings table
  const findings = page.RawFindings || [];
  if (findings.length > 0) {
    const wrap = document.createElement('div');
    wrap.className = 'findings-table-wrap';

    const tableTitle = document.createElement('div');
    tableTitle.className = 'findings-table-title';
    tableTitle.textContent = `Findings (${findings.length})`;
    wrap.appendChild(tableTitle);

    const table = document.createElement('table');
    table.className = 'findings-table';

    table.innerHTML = `
      <thead>
        <tr>
          <th>Label</th>
          <th>Value</th>
          <th>Severity</th>
        </tr>
      </thead>`;

    const tbody = document.createElement('tbody');
    findings.forEach(f => {
      const tr = document.createElement('tr');

      const tdLabel = document.createElement('td');
      tdLabel.textContent = f.Label;

      const tdValue = document.createElement('td');
      tdValue.textContent = f.Value;

      const tdSev = document.createElement('td');
      tdSev.appendChild(renderSeverityBadge(f.Severity));

      tr.appendChild(tdLabel);
      tr.appendChild(tdValue);
      tr.appendChild(tdSev);
      tbody.appendChild(tr);
    });

    table.appendChild(tbody);
    wrap.appendChild(table);
    detail.appendChild(wrap);
  }

  // ── Evidence (if any)
  if (r?.Evidence) {
    const card = document.createElement('div');
    card.className = 'section-card';
    const heading = document.createElement('p');
    heading.className = 'section-heading';
    heading.textContent = 'Evidence';
    const pre = document.createElement('pre');
    pre.className = 'section-content is-code';
    pre.textContent = r.Evidence;
    card.appendChild(heading);
    card.appendChild(pre);
    detail.appendChild(card);
  }

  appRoot.appendChild(detail);
}

// ─────────────────────────────────────────────────────────────────────────────
// Badge helpers  (all use textContent — no innerHTML injection)
// ─────────────────────────────────────────────────────────────────────────────

/**
 * Maps C# TestStatus (int or string) to a CSS class and label.
 * The C# JSON serialiser may emit integers (0,1,2,3) when enums are not
 * configured with JsonStringEnumConverter.
 */
function renderStatusBadge(status) {
  const map = {
    0: ['badge-success', 'Success'],  Success: ['badge-success', 'Success'],
    1: ['badge-failed',  'Failed'],   Failed:  ['badge-failed',  'Failed'],
    2: ['badge-warning', 'Warning'],  Warning: ['badge-warning', 'Warning'],
    3: ['badge-error',   'Error'],    Error:   ['badge-error',   'Error'],
  };
  const [cls, label] = map[status] ?? ['badge-warning', 'Unknown'];
  return makeBadge(cls, label);
}

function renderSeverityBadge(severity) {
  const map = {
    0: ['sev-critical', 'Critical'],  Critical: ['sev-critical', 'Critical'],
    1: ['sev-high',     'High'],      High:     ['sev-high',     'High'],
    2: ['sev-medium',   'Medium'],    Medium:   ['sev-medium',   'Medium'],
    3: ['sev-low',      'Low'],       Low:      ['sev-low',      'Low'],
    4: ['sev-info',     'Info'],      Info:     ['sev-info',     'Info'],
  };
  const [cls, label] = map[severity] ?? ['sev-info', 'Info'];
  return makeBadge(cls, label);
}

/** Renders a plain text badge using a status colour (for BadgeLabel). */
function renderBadgeRaw(text, status) {
  const statusToCls = {
    0: 'badge-success', Success: 'badge-success',
    1: 'badge-failed',  Failed:  'badge-failed',
    2: 'badge-warning', Warning: 'badge-warning',
    3: 'badge-error',   Error:   'badge-error',
  };
  return makeBadge(statusToCls[status] ?? 'badge-warning', text);
}

function makeBadge(cls, label) {
  const badge = document.createElement('span');
  badge.className = `badge ${cls}`;
  const dot = document.createElement('span');
  dot.className = 'badge-dot';
  const txt = document.createTextNode(label);
  badge.appendChild(dot);
  badge.appendChild(txt);
  return badge;
}

function statusTypeLabel(type) {
  // C# TestType enum: 0 = Active, 1 = Passive
  if (type === 0 || type === 'Active')  return 'Active Test';
  if (type === 1 || type === 'Passive') return 'Passive Test';
  return String(type);
}

