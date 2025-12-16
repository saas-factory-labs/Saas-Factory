# 🚂 Railway Deployment

This directory contains complete Railway deployment configuration with automated GitHub Actions workflow.

## 📦 What's Included

- **GitHub Actions Workflow** - Automated deployment pipeline
- **Railway Configuration** - Service and project definitions
- **Setup Scripts** - Automated Railway setup
- **Documentation** - Comprehensive guides and checklists

## 🚀 Quick Start

### 1. Run Setup Script
```powershell
.\setup-railway.ps1
```

### 2. Add GitHub Secrets
In your GitHub repository (Settings → Secrets):
- `RAILWAY_TOKEN_STAGING`
- `RAILWAY_TOKEN_PRODUCTION`
- `RAILWAY_DATABASE_URL_STAGING`
- `RAILWAY_DATABASE_URL_PRODUCTION`

### 3. Deploy!
```bash
git push origin main  # Deploys to staging automatically
```

## 📚 Documentation

| File | Purpose |
|------|---------|
| [RAILWAY_QUICKSTART.md](RAILWAY_QUICKSTART.md) | **Start here!** Quick setup guide |
| [RAILWAY_DEPLOYMENT.md](RAILWAY_DEPLOYMENT.md) | Complete deployment guide |
| [RAILWAY_CHECKLIST.md](RAILWAY_CHECKLIST.md) | Setup and deployment checklists |
| [RAILWAY_OVERVIEW.md](RAILWAY_OVERVIEW.md) | High-level overview |
| [RAILWAY_IMPLEMENTATION_SUMMARY.md](RAILWAY_IMPLEMENTATION_SUMMARY.md) | Technical details |

## 🎯 Deployment Methods

### Automatic (Staging)
Push to `main` → Automatic deployment
```bash
git push origin main
```

### Manual (Production)
1. GitHub → Actions → "Deploy to Railway"
2. Run workflow → Select `production`
3. Approve deployment

### CLI
```powershell
railway up --service api-service --environment staging
```

## ⚙️ Configuration Files

- `railway.json` - Service configuration
- `railway-project.json` - Project structure
- `docker-compose.railway.yml` - Docker overrides
- `.railwayignore` - Deployment optimization

## 🔧 Required Setup

### Railway (One-time)
1. Create Railway account
2. Create project
3. Add PostgreSQL database
4. Configure services
5. Get API tokens

### GitHub (One-time)
1. Add secrets (see above)
2. Configure environment protection
3. Test deployment

## 💰 Estimated Costs

- **Staging**: ~$15-25/month
- **Production**: ~$50-70/month

## 📊 Deployment Flow

```
Push → Build → Test → Deploy API → Migrate DB → Deploy Web → Verify
```

## 🔐 Security

- ✅ GitHub Secrets for tokens
- ✅ Environment protection for production
- ✅ Database backups before migrations
- ✅ Manual approval for production

## 🆘 Need Help?

1. **Quick Start**: Read [RAILWAY_QUICKSTART.md](RAILWAY_QUICKSTART.md)
2. **Setup Issues**: Check [RAILWAY_CHECKLIST.md](RAILWAY_CHECKLIST.md)
3. **Deployment Issues**: See [RAILWAY_DEPLOYMENT.md](RAILWAY_DEPLOYMENT.md) troubleshooting section
4. **Railway Docs**: https://docs.railway.app

## ✅ Next Steps

1. [ ] Read RAILWAY_QUICKSTART.md
2. [ ] Run setup-railway.ps1
3. [ ] Configure GitHub secrets
4. [ ] Test deployment to staging
5. [ ] Deploy to production

---

**Ready to deploy?** Start with [RAILWAY_QUICKSTART.md](RAILWAY_QUICKSTART.md)

