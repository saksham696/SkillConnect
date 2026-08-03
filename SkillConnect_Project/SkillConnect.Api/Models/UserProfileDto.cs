using System.ComponentModel.DataAnnotations;

public class UserProfileDto
{
[Required,Key]
public int Id {get; set;}

public User User {get; set;}

public int UserId { get; set;}
[Required]
public string FullName {get; set;}
[Required]
public string SkillSet {get; set;}
[Required]
public string Experience { get; set;}
[Required]
public string Education {get; set;} 
[Required]
public string ResumePath {get; set;}

public string LinkedInProfile {get; set;}

public string GitHubProfile {get; set;}
[Required]
public string Bio { get; set;}
[Required]
public string Location {get; set;}
[Required]
public string ContactNumber {get; set;}


}