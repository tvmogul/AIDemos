# AINetProfit™ Demos called AIDemos
I have been workig a FREE AI Neural Network Accounting aplication called AINetProfit™ 
that I am getting ready to release. This repository will include parts of that app broken 
up into smaller pieces here in an Electron.NET app called AIDemos. The sample app here will 
include ML.NET code for creating and training various Neural Networks for accounting and 
managing a variety of things from media buying to security.

# Background
My background is in Theoretical Physics and Medicine. After fuinishing medical school I 
decided to sell products I created on National Television instead of practicing. I wrote 
educational software I sold on national television and later I wrote software to manage 
my companies. I used C++ and Bayesian Optimization and an early forms of neural networks 
to determine which television stations to buy half hour infomercial time on. I was able 
to determine which television stations to buy by matching the demographics of a product 
with that of the zip codes from census data that fell within the broadcast radius of each 
television station. And I was able to calculate the maximum amount to bid on each station 
for each half hour of infomercial time to maximize my net profit on each media buy. 
The results of each media buy were feed baack using Bayesian Optimization and that system 
was so successful that it enabled me to make a large fortune.

Later I applied those same techniques to all forms of advertising to determine what ad media 
to buy and the maximum to pay for each buy by calculating the probality of turning a net profit 
over the cost of media.

This series of articles will demonstrate the techniques I developed in AI, Neural Networks 
and Bayesian Optimization.

# Application Structure

The first screen in an accounting app should be what I call the gateway to the database for 
filing tax for some entity which I call a "Company."

Compaanies are the "tax" entities where a company is defined as the entity that files 
tax with the U.S. Governemnt.

Here are examples of tax entities I include as "companies" or entities for filing taxes 
in this AIDemos application where each entity has it's own SQLite database:

<select id="businessType" name="businessType">
    <option value="S Corp">S Corp (Subchapter S Corporation)</option>
    <option value="C Corp">C Corp (C Corporation)</option>
    <option value="Individual">Individual/Sole Proprietorship</option>
    <option value="DBA">DBA (Doing Business As)</option>
    <option value="LLC">LLC (Limited Liability Company)</option>
    <option value="Partnership">Partnership (General Partnership or Limited Partnership)</option>
    <option value="Non-Profit">Non-Profit Organization (501(c)(3), etc.)</option>
    <option value="Trust">Trust</option>
    <option value="Estate">Estate</option>
    <option value="LLP">LLP (Limited Liability Partnership)</option>
    <option value="PC">PC (Professional Corporation)</option>
    <option value="Government">Government Entity</option>
    <option value="Cooperative">Cooperative (Co-op)</option>
    <option value="Joint Venture">Joint Venture</option>
    <option value="Foreign Corp">Foreign Corporation</option>
    <option value="Other">Other</option>
</select>


In the AIDemos app you can create create as many of these Companies ot tax entities as you like.
The first screen forces you to select a "demo" comany or yo can create create a new company before 
you can access any accounting functionality.

Once a database is create for a given tax entity then you can have as many bank accounts or other types 
of accounts for that entity or company.

A company can sell one or more products or services within that tax entity or company  which can also 
just be an individual and the goal is to provide real-world choices where and how to acquire advertising 
bypassing the use of ad agencies that have no motivation to minimize your costs of advertsing.

A special section I will add will apply Neural Network traing and Bayesian Optimization to Per Inquiry (P.I.) 
advertsing where advertsing requires NO out-of-pocket cost upfront. This gurantees a net profit because ad cost 
is a fixed percentage of gross sales after customer recives the product or service.

Also for companies buying ad space we will apply Neural Networks to tageting custoimers via their zip cosdes for 
maximum correlation with their buying habits and using their zip code data to aquire targeted advertising.



