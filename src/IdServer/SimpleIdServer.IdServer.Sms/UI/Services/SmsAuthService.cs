﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SimpleIdServer.IdServer.Sms.UI.Services
{
    public interface ISmsAuthService
    {
        Task<User> Authenticate(string phoneNumber, long otpCode, CancellationToken cancellationToken);
        Task<long> SendCode(string phoneNumber, CancellationToken cancellationToken);
    }

    public class SmsAuthService : ISmsAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;
        private readonly IdServerSmsOptions _smsHostOptions;

        public SmsAuthService(
            IUserRepository userRepository,
            IEnumerable<IOTPAuthenticator> otpAuthenticators,
            IOptions<IdServerSmsOptions> smsHostOptions)
        {
            _userRepository = userRepository;
            _otpAuthenticators = otpAuthenticators;
            _smsHostOptions = smsHostOptions.Value;
        }

        public async Task<User> Authenticate(string phoneNumber, long otpCode, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Query().Include(u => u.Groups).Include(u => u.OAuthUserClaims).Include(u => u.Credentials).FirstOrDefaultAsync(u => u.OAuthUserClaims.Any(c => c.Name == JwtRegisteredClaimNames.PhoneNumber && c.Value == phoneNumber), cancellationToken);
            if (user == null)
                throw new BaseUIException("unknown_phonenumber");

            var activeOtp = user.ActiveOTP;
            if (activeOtp == null)
                throw new BaseUIException("no_active_otp");

            var otpAuthenticator = _otpAuthenticators.Single(a => a.Alg == activeOtp.OTPAlg);
            if (!otpAuthenticator.Verify(otpCode, activeOtp))
                throw new BaseUIException("invalid_confirmationcode");
            return user;
        }

        public async Task<long> SendCode(string phoneNumber, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Query().Include(u => u.Groups).Include(u => u.OAuthUserClaims).Include(u => u.Credentials).FirstOrDefaultAsync(u => u.OAuthUserClaims.Any(c => c.Name == JwtRegisteredClaimNames.PhoneNumber && c.Value == phoneNumber), cancellationToken);
            if (user == null)
                throw new BaseUIException("unknown_phonenumber");

            var activeOtp = user.ActiveOTP;
            if (activeOtp == null)
                throw new BaseUIException("no_active_otp");

            var otpAuthenticator = _otpAuthenticators.Single(a => a.Alg == activeOtp.OTPAlg);
            var otpCode = otpAuthenticator.GenerateOtp(activeOtp);
            TwilioClient.Init(_smsHostOptions.AccountSid, _smsHostOptions.AuthToken);
            await MessageResource.CreateAsync(
                body: string.Format(_smsHostOptions.Message, otpCode),
                from: new PhoneNumber(_smsHostOptions.FromPhoneNumber),
                to: new PhoneNumber(phoneNumber));
            return otpCode;
        }
    }
}
