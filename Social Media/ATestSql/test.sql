
CREATE NONCLUSTERED INDEX [IX_Friendships_RequesterId_Status_Include_AddresseeId]
ON [dbo].[Friendships] ([RequesterId] ASC, [Status] ASC)
INCLUDE [AddresseeId];
GO


CREATE NONCLUSTERED INDEX [IX_Friendships_AddresseeId_Status_Include_RequesterId]
ON [dbo].[Friendships] ([AddresseeId] ASC, [Status] ASC)
INCLUDE ([RequesterId])
GO



DECLARE @UserLoggedInID INT;
DECLARE @PublicPost INT;
DECLARE FriendAccepted INT;

SET @UserLoggedInID = 1;
SET @PublicPost = 1;
SET @FriendAccepted = 1;

SELECT p.Content, p.AuthorId, u.Username AS [AuthorUsername]

	FROM [SocialMediaDB].[dbo].[Posts] AS p

	INNER JOIN [SocialMediaDB].[dbo].[Users] as u ON p.AuthorId = u.Id

	WHERE [AuthorId] = @UserLoggedInID
		OR 
           p.[AuthorId] IN (
			        -- Subquery of User Friends Ids
			        SELECT 
				        CASE
					        WHEN f.RequesterId = @UserLoggedInID THEN f.AddresseeId
					        ELSE f.RequesterId
				        END
			
			        FROM [SocialMediaDB].[dbo].[Friendships] AS f

			        WHERE f.RequesterId = @UserLoggedInID OR f.AddresseeId = @UserLoggedInID

			        AND f.[Status] = @FriendAccepted
		        )
                AND p.Visibility = @PublicPost
            )
	

	SELECT
                [p].[Id],
                [p].[Content],
                [p].[AuthorId],
                [u].[Username] AS[AuthorUsername],
                [p].[CreatedAt]
            FROM[Posts] AS[p]
            -- EF Core automatically joins the Users/ Authors table to grab the Username
            INNER JOIN[Users] AS[u] ON[p].[AuthorId] = [u].[Id]
            WHERE
                -- Condition 1: Posts authored by the logged -in user
                [p].[AuthorId] = 1
                    OR(
                        --Condition 2: Posts where the author is in the subquery of friends
                        [p].[AuthorId] IN(
                            SELECT
                                CASE
                                    WHEN[f].[RequesterId] = 1 THEN[f].[AddresseeId]
                                    ELSE[f].[RequesterId]
                                END
                            FROM[Friendships] AS[f]
                            WHERE
                                ([f].[RequesterId] = 1 OR[f].[AddresseeId] = 1)
                                AND[f].[Status] = 1-- Assuming FriendshipStatus.Accepted maps to enum value 1
                        ) 
                        -- And the post visibility is public
                        AND[p].[Visibility] = 0 -- Assuming PostVisibility.Public maps to enum value 0
                    )
            ORDER BY[p].[CreatedAt] DESC