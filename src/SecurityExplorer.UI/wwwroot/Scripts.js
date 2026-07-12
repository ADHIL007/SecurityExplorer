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
  const noPluginsMsg = document.getElementById('noPluginsMsg');
  
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
    if (testResults.length === 0) {
      pluginsMenu.innerHTML = '<li class="no-plugins">No plugins loaded</li>';
    } else {
      testResults.forEach((result, index) => {
        const li = document.createElement('li');
        const a = document.createElement('a');
        a.innerText = result.TestName || `Plugin ${index + 1}`;
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
    if (result.DashboardWidgetHtml) {
      const wrapper = document.createElement('div');
      wrapper.innerHTML = result.DashboardWidgetHtml;
      grid.appendChild(wrapper);
    } else {
      // Fallback UI if plugin doesn't provide DashboardWidgetHtml
      const card = document.createElement('div');
      card.className = 'default-card';
      
      let statusLabel = result.Status;
      let statusClass = 'status-warn';
      if (statusLabel === 'Pass' || statusLabel === 1) statusClass = 'status-pass';
      if (statusLabel === 'Fail' || statusLabel === 2) statusClass = 'status-fail';

      card.innerHTML = `
        <h3>${result.TestName || 'Unknown Test'}</h3>
        <p>Status: <span class="${statusClass}">${statusLabel}</span></p>
        <p>${result.Description || 'No description provided.'}</p>
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
  
  if (result.PluginDetailsHtml) {
    appRoot.innerHTML = result.PluginDetailsHtml;
  } else {
    appRoot.innerHTML = `
      <div class="header">
        <h1 id="viewTitle">${result.TestName || 'Plugin Details'}</h1>
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
