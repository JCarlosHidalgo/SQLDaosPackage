/*
 * Highlights interface nodes inside Doxygen-generated inheritance graphs.
 *
 * Doxygen 1.17 wraps every dot-generated SVG in an <iframe>. That iframe is
 * a separate document, so the parent's stylesheet does not cascade into it
 * — styling has to be inserted from the inside. This script:
 *
 *   1. Tags interface nodes by inspecting their xlink:href. Doxygen names
 *      interface pages `interface*.html` and class pages `class*.html`.
 *      Hierarchy-graph variants add a `-N-g` suffix (e.g.
 *      `interfaceFoo-1-g.html`), so the name match has to allow `-`.
 *   2. Injects a <style> block into each iframe's SVG document so the rules
 *      that depend on that tag actually apply.
 *   3. Re-injects when the user toggles Doxygen's dark mode.
 */
(function () {
  const SVG_NS = 'http://www.w3.org/2000/svg';
  const XLINK_NS = 'http://www.w3.org/1999/xlink';
  const INTERFACE_HREF_PATTERN = /(^|\/)interface[^/.]+\.html/;
  const STYLE_ID = 'interface-highlight-style';

  /*
   * UML-look (per-class) graphs render each node as several stacked polygons:
   *   - first polygon: the background rectangle (fill="white")
   *   - middle polygons: compartment separator lines (fill="#666666")
   *   - last polygon: the outline border (fill="none", drawn on top of text)
   * The simple hierarchy graph only emits one polygon per node.
   *
   * Filling every polygon paints the outline over the text. So we tint only
   * the first polygon and restyle only the stroke of the last polygon —
   * leaving its fill="none" untouched so the text below stays visible.
   */
  const LIGHT_STYLE = `
    g.node-interface polygon:first-of-type {
      fill: #fef5e7 !important;
      stroke: #c05621 !important;
      stroke-width: 2.5px !important;
      stroke-dasharray: 6 3 !important;
    }
    g.node-interface polygon:last-of-type {
      stroke: #c05621 !important;
      stroke-width: 2.5px !important;
      stroke-dasharray: 6 3 !important;
    }
    g.node-interface text {
      fill: #7c2d12 !important;
      font-style: italic;
      font-weight: bold;
    }
  `;

  const DARK_STYLE = `
    g.node-interface polygon:first-of-type {
      fill: #3b2a0f !important;
      stroke: #f6ad55 !important;
      stroke-width: 2.5px !important;
      stroke-dasharray: 6 3 !important;
    }
    g.node-interface polygon:last-of-type {
      stroke: #f6ad55 !important;
      stroke-width: 2.5px !important;
      stroke-dasharray: 6 3 !important;
    }
    g.node-interface text {
      fill: #fbd38d !important;
      font-style: italic;
      font-weight: bold;
    }
  `;

  function isDarkMode() {
    return document.documentElement.classList.contains('dark-mode');
  }

  function tagInterfaceNodes(svgDoc) {
    svgDoc.querySelectorAll('g.node').forEach(function (node) {
      const link = node.querySelector('a');
      if (!link) return;
      const href =
        link.getAttributeNS(XLINK_NS, 'href') ||
        link.getAttribute('xlink:href') ||
        link.getAttribute('href') ||
        '';
      if (INTERFACE_HREF_PATTERN.test(href)) {
        node.classList.add('node-interface');
      }
    });
  }

  function injectStyle(svgDoc) {
    let style = svgDoc.getElementById(STYLE_ID);
    if (!style) {
      style = svgDoc.createElementNS(SVG_NS, 'style');
      style.setAttribute('id', STYLE_ID);
      svgDoc.documentElement.insertBefore(style, svgDoc.documentElement.firstChild);
    }
    style.textContent = isDarkMode() ? DARK_STYLE : LIGHT_STYLE;
  }

  function processSvgDocument(svgDoc) {
    if (!svgDoc || !svgDoc.documentElement) return;
    if (svgDoc.documentElement.tagName.toLowerCase() !== 'svg') return;
    tagInterfaceNodes(svgDoc);
    injectStyle(svgDoc);
  }

  function attachToIframe(iframe) {
    function handle() {
      try { processSvgDocument(iframe.contentDocument); } catch (_) {}
    }
    iframe.addEventListener('load', handle);
    // Lazy-loaded iframes may not have fired `load` yet, but if the SVG is
    // already there (e.g. after dark-mode toggle re-runs us) process now.
    handle();
  }

  /*
   * Class and interface detail pages wrap the inheritance graph in a
   * collapsible <div class="dynheader closed">. We open it on load so the
   * diagram is visible right away (and the lazy iframe starts loading).
   */
  function expandInheritanceDiagrams(doc) {
    doc.querySelectorAll('iframe[src*="__inherit__graph.svg"]').forEach(function (iframe) {
      const content = iframe.closest('.dyncontent');
      if (!content || !content.id) return;
      const header = doc.getElementById(content.id.replace(/-content$/, ''));
      if (!header || !header.classList.contains('closed')) return;
      // The inline onclick is `return dynsection.toggleVisibility(this)`;
      // a synthetic click keeps the page's own toggle bookkeeping correct.
      header.click();
    });
  }

  function processDocument(doc) {
    expandInheritanceDiagrams(doc);
    doc.querySelectorAll('iframe[src$=".svg"]').forEach(attachToIframe);
    // Legacy <object> path, kept for older Doxygen builds.
    doc.querySelectorAll('object[type="image/svg+xml"], object[data$=".svg"]').forEach(function (obj) {
      function handle() {
        try { processSvgDocument(obj.contentDocument); } catch (_) {}
      }
      obj.addEventListener('load', handle);
      handle();
    });
    // Inline SVGs (also legacy).
    doc.querySelectorAll('svg').forEach(tagInterfaceNodes);
  }

  function refreshAll() {
    document.querySelectorAll('iframe[src$=".svg"]').forEach(function (iframe) {
      try { processSvgDocument(iframe.contentDocument); } catch (_) {}
    });
  }

  new MutationObserver(function (mutations) {
    for (const m of mutations) {
      if (m.attributeName === 'class') {
        refreshAll();
        return;
      }
    }
  }).observe(document.documentElement, { attributes: true });

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function () { processDocument(document); });
  } else {
    processDocument(document);
  }
})();
