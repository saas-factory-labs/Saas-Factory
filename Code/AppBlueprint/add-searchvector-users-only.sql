-- STEP 1: Add SearchVector to Users table
ALTER TABLE "Users" 
ADD COLUMN "SearchVector" tsvector 
GENERATED ALWAYS AS (
    to_tsvector('english', 
        coalesce("FirstName", '') || ' ' || 
        coalesce("LastName", '') || ' ' || 
        coalesce("UserName", '') || ' ' || 
        coalesce("Email", '')
    )
) STORED;

-- STEP 2: Verify it was added
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = 'Users' AND column_name = 'SearchVector';

-- Should show: SearchVector | tsvector
