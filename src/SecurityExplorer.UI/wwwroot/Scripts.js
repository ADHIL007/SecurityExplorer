let testResults = [];
let dashboardHtmlTemplate = '';

// On page load, fetch the dashboard template and then the tests
document.addEventListener('DOMContentLoaded', async () => {
  await loadDashboardTemplate();
  runTests();
});

async function loadDashboardTemplate() {
  try {
    let basePath = window.location.pathname;
    if (basePath.endsWith('/')) basePath = basePath.slice(0, -1);
    if (basePath.endsWith('/index.html')) basePath = basePath.replace('/index.html', '');

    const response = await fetch(`${basePath}/dashboard.html`);
    if (response.ok) {
      dashboardHtmlTemplate = await response.text();
    } else {
      dashboardHtmlTemplate = '<p style="color:var(--alert-red);">Error loading dashboard.html template.</p>';
    }
  } catch (error) {
    dashboardHtmlTemplate = '<p style="color:var(--alert-red);">Error loading dashboard.html template.</p>';
  }
}

async function runTests() {
  // Inject the dashboard shell first so we have the buttons available
  const appRoot = document.getElementById('appRoot');
  appRoot.innerHTML = dashboardHtmlTemplate;

  const btn = document.getElementById('runTestsBtn');
  const content = document.getElementById('contentArea');
  const pluginsMenu = document.getElementById('pluginsMenu');

  if (btn) {
    btn.disabled = true;
    btn.innerText = "Running...";
  }

  try {
    let basePath = window.location.pathname;
    if (basePath.endsWith('/')) basePath = basePath.slice(0, -1);
    if (basePath.endsWith('/index.html')) basePath = basePath.replace('/index.html', '');

    const response = await fetch(`${basePath}/run`);
    if (!response.ok) throw new Error('Network response was not ok');

    testResults = await response.json();

    // Update Sidebar
    pluginsMenu.innerHTML = '';

    // Filter out plugins that don't provide a Page for the sidebar
    const sidebarPlugins = testResults.filter(r => r.Page);

    if (sidebarPlugins.length === 0) {
      pluginsMenu.innerHTML = '<li class="no-plugins">No plugins loaded</li>';
    } else {
      testResults.forEach((result, index) => {
        if (!result.Page) return; // Skip adding to sidebar if no page is provided

        const li = document.createElement('li');
        const a = document.createElement('a');
        a.innerText = result.Result.Title || `Plugin ${index + 1}`;
        a.onclick = () => loadPluginDetails(index);
        a.id = `plugin-link-${index}`;
        li.appendChild(a);
        pluginsMenu.appendChild(li);
      });
    }

    // Load Dashboard View Content
    populateDashboardContent();

  } catch (error) {
    if (content) content.innerHTML = `<p style="color: var(--alert-red);">Error running tests: ${error.message}</p>`;
    pluginsMenu.innerHTML = '<li class="no-plugins">No plugins loaded</li>';
  } finally {
    if (btn) {
      btn.disabled = false;
      btn.innerText = "Run All Tests";
    }
  }
}

function setActiveLink(linkId) {
  document.querySelectorAll('.sidebar-menu a').forEach(a => a.classList.remove('active'));
  if (linkId) {
    const link = document.getElementById(linkId);
    if (link) link.classList.add('active');
  } else {
    const mainLink = document.querySelector('#mainMenu a');
    if (mainLink) mainLink.classList.add('active');
  }
}

function loadDashboard() {
  // Restore the dashboard template to the app root
  document.getElementById('appRoot').innerHTML = dashboardHtmlTemplate;
  populateDashboardContent();
}

function populateDashboardContent() {
  setActiveLink(null);
  const viewTitle = document.getElementById('viewTitle');
  if (viewTitle) viewTitle.innerText = 'Dashboard';

  const content = document.getElementById('contentArea');
  if (!content) return;
  content.innerHTML = '';

  if (testResults.length === 0) {
    content.innerHTML = '<p>No data available.</p>';
    return;
  }

  const grid = document.createElement('div');
  grid.className = 'dashboard-grid';

  testResults.forEach(result => {
    if (result.Widget && result.Widget.DashboardWidgetHtml) {
      const wrapper = document.createElement('div');
      wrapper.innerHTML = result.Widget.DashboardWidgetHtml;
      grid.appendChild(wrapper);
    } else {
      // Fallback UI if plugin doesn't provide DashboardWidgetHtml
      const card = document.createElement('div');
      card.className = 'default-card';

      let statusLabel = result.Result.Status;
      let statusClass = 'status-warn';
      let statusText = 'Unknown';

      // Map C# Enum (which might serialize as int or string)
      if (statusLabel === 0 || statusLabel === 'Success') { statusClass = 'status-pass'; statusText = 'Success'; }
      if (statusLabel === 1 || statusLabel === 'Failed') { statusClass = 'status-fail'; statusText = 'Failed'; }
      if (statusLabel === 2 || statusLabel === 'Warning') { statusClass = 'status-warn'; statusText = 'Warning'; }
      if (statusLabel === 3 || statusLabel === 'Error') { statusClass = 'status-fail'; statusText = 'Error'; }

      card.innerHTML = `
        <h3>${result.Result.Title || 'Unknown Test'}</h3>
        <p>Status: <span class="${statusClass}">${statusText}</span></p>
        <p>${result.Result.Description || 'No description provided.'}</p>
      `;
      grid.appendChild(card);
    }
  });

  content.appendChild(grid);
}

function loadPluginDetails(index) {
  setActiveLink(`plugin-link-${index}`);
  const result = testResults[index];

  const appRoot = document.getElementById('appRoot');

  if (result.Page && result.Page.PluginDetailsHtml) {
    appRoot.innerHTML = result.Page.PluginDetailsHtml;
  } else {
    appRoot.innerHTML = `
      <div class="header">
        <h1 id="viewTitle">${result.Result.Title || 'Plugin Details'}</h1>
      </div>
      <div id="contentArea">
        <div class="default-card">
          <h3>No custom view provided</h3>
          <p>This plugin did not provide a custom PluginDetailsHtml view.</p>
          <pre style="background:#f4f4f4; padding: 10px; border:1px solid #ddd; overflow-x: auto; font-family: monospace;">${JSON.stringify(result, null, 2)}</pre>
        </div>
      </div>
    `;
  }
}
