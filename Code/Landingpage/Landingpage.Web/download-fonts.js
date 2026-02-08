// Download Inter font files for self-hosting
const https = require('https');
const fs = require('fs');
const path = require('path');

const fontsDir = path.join(__dirname, 'wwwroot', 'fonts');
if (!fs.existsSync(fontsDir)) {
  fs.mkdirSync(fontsDir, { recursive: true });
}

// Inter font weights we need
const weights = [300, 400, 500, 600, 700, 800, 900];
const baseUrl = 'https://fonts.gstatic.com/s/inter/v13/';

// Font file mappings (woff2 format for modern browsers)
const fontFiles = {
  300: 'UcCO3FwrK3iLTeHuS_fvQtMwCp50KnMw2boKoduKmMEVuLyfAZ9hiA.woff2',
  400: 'UcCO3FwrK3iLTeHuS_fvQtMwCp50KnMw2boKoduKmMEVuLyfAZJhiA.woff2',
  500: 'UcCO3FwrK3iLTeHuS_fvQtMwCp50KnMw2boKoduKmMEVuLyfAZthiA.woff2',
  600: 'UcCO3FwrK3iLTeHuS_fvQtMwCp50KnMw2boKoduKmMEVuLyfAZVhiA.woff2',
  700: 'UcCO3FwrK3iLTeHuS_fvQtMwCp50KnMw2boKoduKmMEVuLyfAZ9hiA.woff2',
  800: 'UcCO3FwrK3iLTeHuS_fvQtMwCp50KnMw2boKoduKmMEVuLyfAZlhiA.woff2',
  900: 'UcCO3FwrK3iLTeHuS_fvQtMwCp50KnMw2boKoduKmMEVuLyfAZBhiA.woff2'
};

console.log('Downloading Inter font files...');

weights.forEach(weight => {
  const filename = fontFiles[weight];
  const url = baseUrl + filename;
  const outputPath = path.join(fontsDir, `inter-${weight}.woff2`);
  
  https.get(url, (response) => {
    const fileStream = fs.createWriteStream(outputPath);
    response.pipe(fileStream);
    
    fileStream.on('finish', () => {
      fileStream.close();
      console.log(`✓ Downloaded Inter ${weight}`);
    });
  }).on('error', (err) => {
    console.error(`✗ Failed to download Inter ${weight}:`, err.message);
  });
});

// Create fonts.css file
const fontsCss = `
/* Inter Font - Self-Hosted */
@font-face {
  font-family: 'Inter';
  font-style: normal;
  font-weight: 300;
  font-display: swap;
  src: url('/fonts/inter-300.woff2') format('woff2');
}

@font-face {
  font-family: 'Inter';
  font-style: normal;
  font-weight: 400;
  font-display: swap;
  src: url('/fonts/inter-400.woff2') format('woff2');
}

@font-face {
  font-family: 'Inter';
  font-style: normal;
  font-weight: 500;
  font-display: swap;
  src: url('/fonts/inter-500.woff2') format('woff2');
}

@font-face {
  font-family: 'Inter';
  font-style: normal;
  font-weight: 600;
  font-display: swap;
  src: url('/fonts/inter-600.woff2') format('woff2');
}

@font-face {
  font-family: 'Inter';
  font-style: normal;
  font-weight: 700;
  font-display: swap;
  src: url('/fonts/inter-700.woff2') format('woff2');
}

@font-face {
  font-family: 'Inter';
  font-style: normal;
  font-weight: 800;
  font-display: swap;
  src: url('/fonts/inter-800.woff2') format('woff2');
}

@font-face {
  font-family: 'Inter';
  font-style: normal;
  font-weight: 900;
  font-display: swap;
  src: url('/fonts/inter-900.woff2') format('woff2');
}
`;

fs.writeFileSync(path.join(__dirname, 'wwwroot', 'fonts.css'), fontsCss.trim());
console.log('✓ Created fonts.css');
console.log('\nDone! Run "npm run build:css" to compile Tailwind.');
